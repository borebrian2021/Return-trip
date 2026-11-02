using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Return_trip.Models;

namespace Return_trip.Controllers;

[Authorize(Roles = AppRoles.CompanyAgent)]
public class AgentController : Controller
{
    private readonly SystemDBContext _db;
    private readonly UserManager<UserAccount> _userManager;
    private readonly IWebHostEnvironment _environment;

    public AgentController(
        SystemDBContext db,
        UserManager<UserAccount> userManager,
        IWebHostEnvironment environment)
    {
        _db = db;
        _userManager = userManager;
        _environment = environment;
    }

    public IActionResult Dashboard() => RedirectToAction(nameof(Marketplace));

    public async Task<IActionResult> Marketplace()
    {
        var (user, agent) = await GetAgentContextAsync();
        SetLayoutData(user, "marketplace");
        ViewData["Title"] = "Open Cargo";

        var cargoes = await _db.Cargoes
            .AsNoTracking()
            .Where(c => c.Status == "Open" && c.IsBidOpen)
            .Include(cargo => cargo.Bids)
            .OrderByDescending(c => c.CreatedAt)
            .Take(100)
            .ToListAsync();

        var routes = await _db.VehicleRoutes
            .AsNoTracking()
            .Where(route => route.VehicleDetail.VehicleOwnerId == agent.VehicleOwnerId &&
                            (!agent.CompanyId.HasValue || route.VehicleDetail.CompanyId == agent.CompanyId.Value) &&
                            route.FromLatitude.HasValue && route.FromLongitude.HasValue &&
                            route.ToLatitude.HasValue && route.ToLongitude.HasValue)
            .Include(route => route.VehicleDetail)
                .ThenInclude(vehicle => vehicle.Company)
            .ToListAsync();

        var latestRoutes = routes
            .GroupBy(route => route.VehicleDetailId)
            .Select(group => group.OrderByDescending(route => route.UpdatedAt ?? route.CreatedAt).First())
            .ToList();
        var routesAwaitingDeposit = await _db.CargoBids
            .AsNoTracking()
            .Where(bid => bid.Status == "Accepted" && bid.PaymentStatus == "AwaitingDeposit")
            .Select(bid => bid.VehicleRouteId)
            .ToListAsync();
        latestRoutes = latestRoutes
            .Where(route => !routesAwaitingDeposit.Contains(route.Id))
            .ToList();

        var existingBids = await _db.CargoBids
            .AsNoTracking()
            .Where(bid => bid.CompanyAgentId == agent.Id && cargoes.Select(cargo => cargo.Id).Contains(bid.CargoId))
            .ToDictionaryAsync(bid => bid.CargoId);

        var model = new AgentMarketplaceViewModel
        {
            Listings = cargoes.Select(cargo =>
            {
                existingBids.TryGetValue(cargo.Id, out var existingBid);
                var eligibleVehicles = latestRoutes
                    .Select(route => CreateVehicleOption(route, cargo))
                    .Where(option => option != null)
                    .Select(option => option!)
                    .ToList();

                return new AgentCargoOpportunityViewModel
                {
                    CargoId = cargo.Id,
                    Title = cargo.Title,
                    Description = cargo.Description,
                    PackageType = cargo.PackageType,
                    WeightKg = cargo.WeightKg,
                    FromLocation = cargo.FromLocation,
                    ToLocation = cargo.ToLocation,
                    TransportCost = cargo.TransportCost,
                    PricePerKg = cargo.PricePerKg,
                    PickupDateTime = cargo.PickupDateTime ?? cargo.RequiredDepartureAt,
                    PickupDeadlineAt = cargo.PickupDeadlineAt,
                    DeliveryDeadlineAt = cargo.DeliveryDeadlineAt,
                    BidCount = cargo.Bids.Count,
                    ExistingBidId = existingBid?.Id,
                    ExistingBidStatus = existingBid?.Status,
                    ExistingPriceSuggestionStatus = existingBid?.PriceSuggestionStatus,
                    BidForm = new AgentPlaceBidViewModel
                    {
                        CargoId = cargo.Id,
                        BidAmount = existingBid?.BidAmount ?? cargo.TransportCost,
                        SuggestedAmount = existingBid?.SuggestedAmount,
                        SuggestionComments = existingBid?.SuggestionComments,
                        VehicleRouteId = existingBid?.VehicleRouteId ?? eligibleVehicles.FirstOrDefault()?.VehicleRouteId ?? 0,
                    },
                    EligibleVehicles = eligibleVehicles,
                };
            }).ToList(),
        };

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceBid(AgentPlaceBidViewModel model)
    {
        var (user, agent) = await GetAgentContextAsync();
        SetLayoutData(user, "marketplace");
        ViewData["Title"] = "Open Cargo";

        if (model.SuggestedAmount.HasValue && string.IsNullOrWhiteSpace(model.SuggestionComments))
        {
            ModelState.AddModelError(nameof(model.SuggestionComments), "Explain why you are suggesting a different transport price.");
        }

        var cargo = await _db.Cargoes
            .FirstOrDefaultAsync(c => c.Id == model.CargoId && c.Status == "Open" && c.IsBidOpen);
        if (cargo == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Check the bid amount and price suggestion details, then try again.";
            return RedirectToAction(nameof(Marketplace));
        }

        var route = await _db.VehicleRoutes
            .Include(vehicleRoute => vehicleRoute.VehicleDetail)
                .ThenInclude(vehicle => vehicle.Company)
            .FirstOrDefaultAsync(vehicleRoute =>
                vehicleRoute.Id == model.VehicleRouteId &&
                vehicleRoute.IsAwaitingReturn &&
                vehicleRoute.VehicleDetail.VehicleOwnerId == agent.VehicleOwnerId &&
                (!agent.CompanyId.HasValue || vehicleRoute.VehicleDetail.CompanyId == agent.CompanyId.Value));

        if (route == null || CreateVehicleOption(route, cargo) == null)
        {
            TempData["Error"] = "That vehicle is no longer eligible for this cargo. Refresh the cargo board and choose an available vehicle.";
            return RedirectToAction(nameof(Marketplace));
        }
        var latestRouteId = await _db.VehicleRoutes
            .Where(vehicleRoute => vehicleRoute.VehicleDetailId == route.VehicleDetailId)
            .OrderByDescending(vehicleRoute => vehicleRoute.UpdatedAt ?? vehicleRoute.CreatedAt)
            .Select(vehicleRoute => vehicleRoute.Id)
            .FirstAsync();
        if (latestRouteId != route.Id)
        {
            TempData["Error"] = "That route has been replaced. Refresh the cargo board and select the vehicle's current route.";
            return RedirectToAction(nameof(Marketplace));
        }

        var vehicleIsReserved = await _db.CargoBids.AnyAsync(bid =>
            bid.VehicleRouteId == route.Id &&
            bid.Status == "Accepted" &&
            (bid.PaymentStatus == "AwaitingDeposit" ||
             (bid.DepositPaidAt.HasValue && bid.Cargo.DeliveredAt == null)));
        if (vehicleIsReserved)
        {
            TempData["Error"] = "That vehicle is already held for an awarded or paid cargo booking.";
            return RedirectToAction(nameof(Marketplace));
        }

        var bid = await _db.CargoBids
            .FirstOrDefaultAsync(existing => existing.CargoId == cargo.Id && existing.CompanyAgentId == agent.Id);
        if (bid?.Status == "Accepted")
        {
            TempData["Error"] = "Your bid for this cargo has already been accepted.";
            return RedirectToAction(nameof(Marketplace));
        }

        if (bid == null)
        {
            bid = new CargoBid
            {
                CargoId = cargo.Id,
                CompanyAgentId = agent.Id,
                CreatedAt = DateTime.UtcNow,
            };
            _db.CargoBids.Add(bid);
        }

        bid.VehicleRouteId = route.Id;
        bid.BidAmount = model.BidAmount;
        bid.SuggestedAmount = model.SuggestedAmount;
        bid.SuggestionComments = model.SuggestedAmount.HasValue ? model.SuggestionComments?.Trim() : null;
        bid.PriceSuggestionStatus = model.SuggestedAmount.HasValue ? "Pending" : "NotRequired";
        bid.PriceDecisionAt = null;
        bid.PriceDecidedByGoodsOwnerId = null;
        bid.Status = "Pending";
        bid.PaymentStatus = "NotStarted";
        bid.DepositAmount = null;
        bid.DepositPaidAt = null;
        bid.PaymentReference = null;

        await _db.SaveChangesAsync();
        TempData["Success"] = model.SuggestedAmount.HasValue
            ? "Your bid and price suggestion have been sent to the goods owner for review."
            : "Your bid has been submitted to the goods owner.";
        return RedirectToAction(nameof(Bids));
    }

    public async Task<IActionResult> Bids()
    {
        var (user, agent) = await GetAgentContextAsync();
        SetLayoutData(user, "bids");
        ViewData["Title"] = "My Bids";

        var bids = await _db.CargoBids
            .AsNoTracking()
            .Where(bid => bid.CompanyAgentId == agent.Id)
            .Include(bid => bid.Cargo)
            .Include(bid => bid.VehicleRoute)
                .ThenInclude(route => route.VehicleDetail)
            .OrderByDescending(bid => bid.CreatedAt)
            .ToListAsync();

        return View(new AgentBidsPageViewModel
        {
            Bids = bids.Select(bid => new AgentBidListItemViewModel
            {
                BidId = bid.Id,
                CargoTitle = bid.Cargo.Title,
                CargoRoute = $"{bid.Cargo.FromLocation} → {bid.Cargo.ToLocation}",
                VehicleRegistration = bid.VehicleRoute.VehicleDetail.RegistrationNumber,
                BidAmount = bid.BidAmount,
                SuggestedAmount = bid.SuggestedAmount,
                BidStatus = bid.Status,
                PriceSuggestionStatus = bid.PriceSuggestionStatus,
                PaymentStatus = bid.PaymentStatus,
                DepositAmount = bid.DepositAmount,
                CreatedAt = bid.CreatedAt,
            }).ToList(),
        });
    }

    public async Task<IActionResult> Trips(CancellationToken cancellationToken)
    {
        var (user, agent) = await GetAgentContextAsync();
        SetLayoutData(user, "trips");
        ViewData["Title"] = "Trip Verification";

        var bids = await _db.CargoBids
            .Where(bid => bid.CompanyAgentId == agent.Id && bid.Status == "Accepted")
            .Include(bid => bid.Cargo)
            .Include(bid => bid.Driver)
            .Include(bid => bid.VehicleRoute)
                .ThenInclude(route => route.VehicleDetail)
                    .ThenInclude(vehicle => vehicle.Drivers)
            .Include(bid => bid.Payments)
            .Include(bid => bid.FuelAdvances)
            .Include(bid => bid.DeliveryReceipt)
            .OrderByDescending(bid => bid.AcceptedAt)
            .ToListAsync(cancellationToken);

        var overdueFuelIds = bids
            .SelectMany(bid => bid.FuelAdvances)
            .Where(fuel => fuel.Status == "PendingReconciliation" && DateTime.UtcNow > fuel.PaidAt.AddDays(3))
            .Select(fuel => fuel.Id)
            .ToList();
        if (overdueFuelIds.Count > 0)
        {
            var alreadyNotified = await _db.Notifications
                .Where(notification =>
                    notification.UserAccountId == user.Id &&
                    notification.RelatedEntityType == "FuelAdvanceOverdue" &&
                    notification.RelatedEntityId.HasValue &&
                    overdueFuelIds.Contains(notification.RelatedEntityId.Value))
                .Select(notification => notification.RelatedEntityId!.Value)
                .ToListAsync(cancellationToken);
            foreach (var fuelId in overdueFuelIds.Except(alreadyNotified))
            {
                var fuel = bids.SelectMany(bid => bid.FuelAdvances).Single(item => item.Id == fuelId);
                _db.Notifications.Add(new Notification
                {
                    UserAccountId = user.Id,
                    Title = "Fuel reconciliation overdue",
                    Message = $"Fuel payment reference {fuel.Reference} for trip RT-{fuel.CargoBidId:D6} passed its three-day reconciliation window.",
                    Type = "Trip",
                    RelatedEntityType = "FuelAdvanceOverdue",
                    RelatedEntityId = fuel.Id,
                });
            }
            if (overdueFuelIds.Except(alreadyNotified).Any())
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        return View(new AgentTripsViewModel
        {
            Trips = bids.Select(bid =>
            {
                var deposit = bid.PaymentStatus == "Paid" && bid.DepositPaidAt.HasValue
                    ? bid.DepositAmount ?? 0m
                    : 0m;
                var fuel = bid.FuelAdvances.Where(item => item.Status == "Reconciled").Sum(item => item.Amount);
                var finalPayments = bid.Payments.Where(item => item.Status == "Confirmed").Sum(item => item.Amount);
                return new AgentTripRowViewModel
                {
                    BidId = bid.Id,
                    Reference = $"RT-{bid.Id:D6}",
                    CargoTitle = bid.Cargo.Title,
                    DriverName = bid.Driver?.FullName ?? "Not assigned",
                    DriverId = bid.DriverId,
                    TripStatus = bid.TripStatus,
                    PaymentStatus = bid.PaymentStatus,
                    TripCost = bid.BidAmount,
                    Deposit = deposit,
                    ReconciledFuel = fuel,
                    ConfirmedFinalPayments = finalPayments,
                    Outstanding = Math.Max(0m, bid.BidAmount - deposit - fuel - finalPayments),
                    DeliveryReceiptId = bid.DeliveryReceipt?.Id,
                    DeliveryEvidenceName = bid.DeliveryReceipt?.ImageFileName,
                    ReceiptStatus = bid.DeliveryReceipt?.ConfirmationStatus,
                    Drivers = bid.VehicleRoute.VehicleDetail.Drivers
                        .Where(driver => driver.UserAccountId.HasValue)
                        .Select(driver => new AgentDriverOptionViewModel { Id = driver.Id, Name = driver.FullName })
                        .ToList(),
                    PendingPayments = bid.Payments.Where(payment => payment.Status == "PendingVerification").ToList(),
                    FuelAdvances = bid.FuelAdvances.ToList(),
                };
            }).ToList(),
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignDriver(AssignDriverViewModel model, CancellationToken cancellationToken)
    {
        var (_, agent) = await GetAgentContextAsync();
        var bid = await _db.CargoBids
            .Include(item => item.VehicleRoute)
            .Include(item => item.Driver)
            .FirstOrDefaultAsync(item =>
                item.Id == model.BidId &&
                item.CompanyAgentId == agent.Id &&
                item.Status == "Accepted" &&
                (item.TripStatus == "AwaitingDeposit" || item.TripStatus == "ReadyForPickup"),
                cancellationToken);
        if (bid == null) return NotFound();

        var driver = await _db.Drivers.FirstOrDefaultAsync(item =>
            item.Id == model.DriverId &&
            item.VehicleDetailId == bid.VehicleRoute.VehicleDetailId &&
            item.VehicleOwnerId == agent.VehicleOwnerId &&
            item.UserAccountId.HasValue,
            cancellationToken);
        if (driver == null)
        {
            TempData["Error"] = "Select an active driver account assigned to this trip's vehicle.";
            return RedirectToAction(nameof(Trips));
        }

        bid.DriverId = driver.Id;
        bid.TripAssignedAt = DateTime.UtcNow;
        bid.TripStatus = bid.PaymentStatus == "Paid" && bid.DepositPaidAt.HasValue
            ? "ReadyForPickup"
            : "AwaitingDeposit";
        _db.TripActivities.Add(new TripActivity
        {
            CargoBidId = bid.Id,
            UserAccountId = (await _userManager.GetUserAsync(User))?.Id,
            Activity = "DriverAssigned",
            Details = $"Driver {driver.FullName} assigned to trip RT-{bid.Id:D6}.",
        });
        if (driver.UserAccountId is int driverAccountId)
        {
            _db.Notifications.Add(new Notification
            {
                UserAccountId = driverAccountId,
                Title = "Trip assigned",
                Message = $"You have been assigned trip RT-{bid.Id:D6}. Receiver contacts unlock after the deposit is confirmed.",
                Type = "Trip",
                RelatedEntityType = "CargoBid",
                RelatedEntityId = bid.Id,
            });
        }
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Driver assigned to trip.";
        return RedirectToAction(nameof(Trips));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordFuelAdvance(AddFuelAdvanceViewModel model, CancellationToken cancellationToken)
    {
        var (user, agent) = await GetAgentContextAsync();
        var paidAtUtc = model.PaidAt.Kind == DateTimeKind.Utc
            ? model.PaidAt
            : DateTime.SpecifyKind(model.PaidAt, DateTimeKind.Local).ToUniversalTime();
        if (!ModelState.IsValid || paidAtUtc > DateTime.UtcNow)
        {
            TempData["Error"] = "Enter a valid fuel payment amount, reference, and payment date.";
            return RedirectToAction(nameof(Trips));
        }

        var bid = await _db.CargoBids
            .Include(item => item.FuelAdvances)
            .Include(item => item.Payments)
            .FirstOrDefaultAsync(item =>
                item.Id == model.BidId &&
                item.CompanyAgentId == agent.Id &&
                item.Status == "Accepted" &&
                item.DepositPaidAt.HasValue &&
                (item.TripStatus == "ReadyForPickup" ||
                 item.TripStatus == "CargoPickedUp" ||
                 item.TripStatus == "InTransit"),
                cancellationToken);
        if (bid == null) return NotFound();

        var reference = model.Reference.Trim();
        if (await _db.FuelAdvances.AnyAsync(item => item.Reference == reference, cancellationToken))
        {
            TempData["Error"] = "That fuel transaction reference has already been recorded.";
            return RedirectToAction(nameof(Trips));
        }

        var deposit = bid.PaymentStatus == "Paid" ? bid.DepositAmount ?? 0m : 0m;
        var existingFuel = bid.FuelAdvances.Sum(item => item.Amount);
        var finalPayments = bid.Payments.Sum(item => item.Amount);
        if (existingFuel + finalPayments + model.Amount > bid.BidAmount - deposit)
        {
            TempData["Error"] = "The fuel amount exceeds the remaining trip balance and cannot be recorded.";
            return RedirectToAction(nameof(Trips));
        }

        var fuel = new FuelAdvance
        {
            CargoBidId = bid.Id,
            Amount = model.Amount,
            Reference = reference,
            PaidAt = paidAtUtc,
            Status = "PendingReconciliation",
        };
        _db.FuelAdvances.Add(fuel);
        _db.TripActivities.Add(new TripActivity
        {
            CargoBidId = bid.Id,
            UserAccountId = user.Id,
            Activity = "FuelAdvanceRecorded",
            Details = $"Advance fuel amount KSh {model.Amount:N2}, reference {reference}; reconcile within three days of payment.",
        });
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Fuel payment recorded separately and flagged for reconciliation.";
        return RedirectToAction(nameof(Trips));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReconcileFuel(int id, CancellationToken cancellationToken)
    {
        var (user, agent) = await GetAgentContextAsync();
        var fuel = await _db.FuelAdvances
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.FuelAdvances)
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.Cargo)
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.Driver)
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.Payments)
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.VehicleRoute)
            .FirstOrDefaultAsync(item =>
                item.Id == id &&
                item.CargoBid.CompanyAgentId == agent.Id &&
                item.CargoBid.Status == "Accepted",
                cancellationToken);
        if (fuel == null) return NotFound();
        if (fuel.Status != "PendingReconciliation" || !fuel.CargoBid.DepositPaidAt.HasValue)
        {
            TempData["Error"] = "This fuel payment is no longer available for reconciliation.";
            return RedirectToAction(nameof(Trips));
        }

        var bid = fuel.CargoBid;
        var deposit = bid.PaymentStatus == "Paid" ? bid.DepositAmount ?? 0m : 0m;
        var otherFuelCredits = bid.FuelAdvances
            .Where(item => item.Id != fuel.Id)
            .Sum(item => item.Amount);
        var confirmedFinalPayments = await _db.CargoPayments
            .Where(item => item.CargoBidId == bid.Id && item.Status == "Confirmed")
            .SumAsync(item => (decimal?)item.Amount, cancellationToken) ?? 0m;
        if (otherFuelCredits + fuel.Amount + confirmedFinalPayments > bid.BidAmount - deposit)
        {
            TempData["Error"] = "The fuel adjustment exceeds the remaining trip amount and cannot be approved.";
            return RedirectToAction(nameof(Trips));
        }

        fuel.Status = "Reconciled";
        fuel.ReconciledByCompanyAgentId = agent.Id;
        fuel.ReconciledAt = DateTime.UtcNow;
        var late = DateTime.UtcNow > fuel.PaidAt.AddDays(3);
        var currentDeposit = bid.PaymentStatus == "Paid" ? bid.DepositAmount ?? 0m : 0m;
        var reconciledFuelTotal = bid.FuelAdvances
            .Where(item => item.Status == "Reconciled")
            .Sum(item => item.Amount);
        var confirmedFinalTotal = bid.Payments
            .Where(item => item.Status == "Confirmed")
            .Sum(item => item.Amount);
        var fullySettled = currentDeposit + reconciledFuelTotal + confirmedFinalTotal >= bid.BidAmount;
        if (fullySettled && (bid.TripStatus is "ArrivedAtDestination" or "AwaitingFinalPayment"))
        {
            bid.TripStatus = "PaymentConfirmed";
        }
        _db.TripActivities.Add(new TripActivity
        {
            CargoBidId = bid.Id,
            UserAccountId = user.Id,
            Activity = "FuelReconciliationApproved",
            Details = $"Fuel adjustment KSh {fuel.Amount:N2} approved{(late ? " after the three-day reconciliation window" : " within the three-day reconciliation window")}.",
        });
        var ownerAccountId = await _db.GoodsOwners
            .Where(owner => owner.Id == bid.Cargo.GoodsOwnerId)
            .Select(owner => owner.UserAccountId)
            .SingleAsync(cancellationToken);
        _db.Notifications.Add(new Notification
        {
            UserAccountId = ownerAccountId,
            Title = "Fuel adjustment reconciled",
            Message = $"Fuel advance for trip RT-{bid.Id:D6} was reconciled. The verified amount is deducted from the final balance.",
            Type = "Trip",
            RelatedEntityType = "CargoBid",
            RelatedEntityId = bid.Id,
        });
        if (bid.Driver?.UserAccountId is int driverAccountId)
        {
            _db.Notifications.Add(new Notification
            {
                UserAccountId = driverAccountId,
                Title = fullySettled ? "Trip balance settled" : "Fuel adjustment reconciled",
                Message = fullySettled
                    ? $"The fuel adjustment for trip RT-{bid.Id:D6} settled the full applicable balance. Cargo handover may proceed."
                    : $"The applicable fuel adjustment for trip RT-{bid.Id:D6} was approved.",
                Type = "Trip",
                RelatedEntityType = "CargoBid",
                RelatedEntityId = bid.Id,
            });
        }
        await _db.SaveChangesAsync(cancellationToken);
        TempData[late ? "Error" : "Success"] = late
            ? "Late fuel reconciliation approved and recorded for agent attention."
            : "Fuel adjustment reconciled. The verified amount is deducted from the final balance.";
        return RedirectToAction(nameof(Trips));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyFinalPayment(
        VerifyPaymentViewModel model,
        bool approve,
        CancellationToken cancellationToken)
    {
        var (user, agent) = await GetAgentContextAsync();
        if (!approve && string.IsNullOrWhiteSpace(model.Comments))
        {
            TempData["Error"] = "Add a reason before rejecting a payment reference.";
            return RedirectToAction(nameof(Trips));
        }

        var payment = await _db.CargoPayments
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.Payments)
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.FuelAdvances)
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.Cargo)
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.Driver)
            .Include(item => item.CargoBid)
                .ThenInclude(bid => bid.VehicleRoute)
            .FirstOrDefaultAsync(item =>
                item.Id == model.PaymentId &&
                item.CargoBid.CompanyAgentId == agent.Id,
                cancellationToken);
        if (payment == null) return NotFound();
        if (payment.Status != "PendingVerification")
        {
            TempData["Error"] = "This payment has already been reviewed.";
            return RedirectToAction(nameof(Trips));
        }

        var bid = payment.CargoBid;
        if (bid.Status != "Accepted" ||
            bid.TripStatus != "AwaitingFinalPayment" ||
            !bid.DepositPaidAt.HasValue ||
            bid.PaymentStatus != "Paid")
        {
            TempData["Error"] = "This trip is not in a state that permits final payment verification.";
            return RedirectToAction(nameof(Trips));
        }

        if (approve)
        {
            var deposit = bid.PaymentStatus == "Paid" ? bid.DepositAmount ?? 0m : 0m;
            var fuel = bid.FuelAdvances
                .Where(item => item.Status == "Reconciled")
                .Sum(item => item.Amount);
            var otherConfirmedPayments = bid.Payments
                .Where(item => item.Id != payment.Id && item.Status == "Confirmed")
                .Sum(item => item.Amount);
            var remainingBeforeThisPayment = Math.Max(0m, bid.BidAmount - deposit - fuel - otherConfirmedPayments);
            if (payment.Amount > remainingBeforeThisPayment ||
                bid.FuelAdvances.Any(item => item.Status != "Reconciled"))
            {
                TempData["Error"] = "Payment cannot be approved until the verified balance and all fuel adjustments reconcile.";
                return RedirectToAction(nameof(Trips));
            }

            payment.Status = "Confirmed";
            payment.VerifiedByCompanyAgentId = agent.Id;
            payment.VerifiedAt = DateTime.UtcNow;
            payment.VerificationComments = model.Comments?.Trim();
            var fullyPaid = payment.Amount == remainingBeforeThisPayment;
            if (fullyPaid)
            {
                bid.TripStatus = "PaymentConfirmed";
                if (bid.Driver?.UserAccountId is int driverAccountId)
                {
                    _db.Notifications.Add(new Notification
                    {
                        UserAccountId = driverAccountId,
                        Title = "Final payment confirmed",
                        Message = $"The full balance for trip RT-{bid.Id:D6} is confirmed. Cargo handover may proceed.",
                        Type = "Trip",
                        RelatedEntityType = "CargoBid",
                        RelatedEntityId = bid.Id,
                    });
                }
            }
        }
        else
        {
            payment.Status = "Rejected";
            payment.VerifiedByCompanyAgentId = agent.Id;
            payment.VerifiedAt = DateTime.UtcNow;
            payment.VerificationComments = string.IsNullOrWhiteSpace(model.Comments)
                ? "Payment reference could not be verified."
                : model.Comments.Trim();
            bid.TripStatus = "ArrivedAtDestination";
        }

        _db.TripActivities.Add(new TripActivity
        {
            CargoBidId = bid.Id,
            UserAccountId = user.Id,
            Activity = approve ? "FinalPaymentConfirmed" : "FinalPaymentRejected",
            Details = $"Payment reference {payment.Reference} was {(approve ? "confirmed" : "rejected")}. {payment.VerificationComments}",
        });
        var ownerAccountId = await _db.GoodsOwners
            .Where(owner => owner.Id == bid.Cargo.GoodsOwnerId)
            .Select(owner => owner.UserAccountId)
            .SingleAsync(cancellationToken);
        _db.Notifications.Add(new Notification
        {
            UserAccountId = ownerAccountId,
            Title = approve ? "Final payment verified" : "Payment needs attention",
            Message = approve
                ? $"Your final payment for trip RT-{bid.Id:D6} has been verified."
                : $"Your payment for trip RT-{bid.Id:D6} was not verified. {payment.VerificationComments}",
            Type = "Trip",
            RelatedEntityType = "CargoBid",
            RelatedEntityId = bid.Id,
        });
        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = approve ? "Final payment verified." : "Payment rejected; the goods owner has been notified.";
        return RedirectToAction(nameof(Trips));
    }

    [HttpGet]
    public async Task<IActionResult> DeliveryEvidence(int id, CancellationToken cancellationToken)
    {
        var (_, agent) = await GetAgentContextAsync();
        var receipt = await _db.DeliveryReceipts
            .AsNoTracking()
            .Include(item => item.CargoBid)
            .FirstOrDefaultAsync(item =>
                item.Id == id &&
                item.CargoBid.CompanyAgentId == agent.Id,
                cancellationToken);
        if (receipt == null) return NotFound();

        var filePath = Path.Combine(
            _environment.ContentRootPath,
            "App_Data",
            "DeliveryEvidence",
            Path.GetFileName(receipt.ImageUrl));
        if (!System.IO.File.Exists(filePath)) return NotFound();
        return PhysicalFile(filePath, receipt.ImageContentType, receipt.ImageFileName ?? "delivery-evidence");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReviewDelivery(
        int id,
        bool approve,
        string? comments,
        CancellationToken cancellationToken)
    {
        var (user, agent) = await GetAgentContextAsync();
        if (!approve && string.IsNullOrWhiteSpace(comments))
        {
            TempData["Error"] = "Add a reason before rejecting delivery evidence.";
            return RedirectToAction(nameof(Trips));
        }

        var strategy = _db.Database.CreateExecutionStrategy();
        var result = "not-found";
        await strategy.ExecuteAsync(async () =>
        {
            _db.ChangeTracker.Clear();
            await using var transaction = await _db.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable,
                cancellationToken);
            var receipt = await _db.DeliveryReceipts
                .Include(item => item.CargoBid)
                    .ThenInclude(bid => bid.Cargo)
                .Include(item => item.CargoBid)
                    .ThenInclude(bid => bid.Payments)
                .Include(item => item.CargoBid)
                    .ThenInclude(bid => bid.FuelAdvances)
                .Include(item => item.CargoBid)
                    .ThenInclude(bid => bid.Driver)
                .Include(item => item.CargoBid)
                    .ThenInclude(bid => bid.VehicleRoute)
                .FirstOrDefaultAsync(item =>
                    item.Id == id &&
                    item.CargoBid.CompanyAgentId == agent.Id,
                    cancellationToken);
            if (receipt == null) return;
            result = "not-pending";
            var bid = receipt.CargoBid;
            if (receipt.ConfirmationStatus != "Pending" ||
                bid.TripStatus != "AwaitingAgentVerification" ||
                bid.Status != "Accepted" ||
                bid.DriverId != receipt.DriverId)
            {
                return;
            }

            if (!approve)
            {
                receipt.ConfirmationStatus = "Rejected";
                receipt.ConfirmedByCompanyAgentId = agent.Id;
                receipt.ConfirmedAt = DateTime.UtcNow;
                receipt.AgentComments = string.IsNullOrWhiteSpace(comments)
                    ? "Delivery evidence was rejected."
                    : comments.Trim();
                bid.TripStatus = "PaymentConfirmed";
                _db.TripActivities.Add(new TripActivity
                {
                    CargoBidId = bid.Id,
                    UserAccountId = user.Id,
                    Activity = "DeliveryEvidenceRejected",
                    Details = receipt.AgentComments,
                });
                if (bid.Driver?.UserAccountId is int driverAccountId)
                {
                    _db.Notifications.Add(new Notification
                    {
                        UserAccountId = driverAccountId,
                        Title = "Delivery evidence rejected",
                        Message = $"Delivery evidence for trip RT-{bid.Id:D6} was rejected. {receipt.AgentComments}",
                        Type = "Trip",
                        RelatedEntityType = "CargoBid",
                        RelatedEntityId = bid.Id,
                    });
                }
                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                result = "rejected";
                return;
            }

            var evidencePath = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "DeliveryEvidence",
                Path.GetFileName(receipt.ImageUrl));
            if (!System.IO.File.Exists(evidencePath))
            {
                result = "evidence-missing";
                return;
            }

            var deposit = bid.PaymentStatus == "Paid" ? bid.DepositAmount ?? 0m : 0m;
            var fuel = bid.FuelAdvances.Where(item => item.Status == "Reconciled").Sum(item => item.Amount);
            var finalPayments = bid.Payments.Where(item => item.Status == "Confirmed").Sum(item => item.Amount);
            if (bid.TripStatus != "AwaitingAgentVerification" ||
                bid.DepositPaidAt == null ||
                bid.PaymentStatus != "Paid" ||
                bid.FuelAdvances.Any(item => item.Status != "Reconciled") ||
                Math.Max(0m, bid.BidAmount - deposit - fuel - finalPayments) != 0m)
            {
                result = "financial-check-failed";
                return;
            }

            var now = DateTime.UtcNow;
            receipt.ConfirmationStatus = "Confirmed";
            receipt.ConfirmedByCompanyAgentId = agent.Id;
            receipt.ConfirmedAt = now;
            receipt.AgentComments = comments?.Trim();
            bid.TripStatus = "Closed";
            bid.Status = "Completed";
            bid.CompletedAt = now;
            bid.Cargo.DeliveredAt = receipt.SignedAt;
            bid.Cargo.Status = "Closed";
            bid.VehicleRoute.IsAwaitingReturn = true;
            bid.VehicleRoute.ReturnAvailabilityStartsAt = receipt.SignedAt;
            bid.VehicleRoute.ReturnAvailabilityEndsAt = null;
            bid.VehicleRoute.UpdatedAt = now;
            if (bid.Driver != null)
            {
                bid.Driver.LastCargoTransported =
                    $"{bid.Cargo.Title} ({bid.Cargo.PackageType}, {bid.Cargo.WeightKg:N0} kg) delivered to {bid.Cargo.ToLocation}; {now:yyyy-MM-dd} · RT-{bid.Id:D6}";
                bid.Driver.LastDeliveryAt = receipt.SignedAt;
                bid.Driver.LastTripReference = $"RT-{bid.Id:D6}";
                bid.Driver.LastCargoUpdatedAt = now;
            }
            _db.TripActivities.Add(new TripActivity
            {
                CargoBidId = bid.Id,
                UserAccountId = user.Id,
                Activity = "TripClosed",
                Details = "Delivery and financial records approved; driver cargo profile updated before trip closure.",
                CreatedAt = now,
            });

            var ownerAccountId = await _db.GoodsOwners
                .Where(owner => owner.Id == bid.Cargo.GoodsOwnerId)
                .Select(owner => owner.UserAccountId)
                .SingleAsync(cancellationToken);
            _db.Notifications.Add(new Notification
            {
                UserAccountId = ownerAccountId,
                Title = "Trip completed",
                Message = $"Trip RT-{bid.Id:D6} has been verified and closed.",
                Type = "Trip",
                RelatedEntityType = "CargoBid",
                RelatedEntityId = bid.Id,
            });
            if (bid.Driver?.UserAccountId is int assignedAccountId)
            {
                _db.Notifications.Add(new Notification
                {
                    UserAccountId = assignedAccountId,
                    Title = "Trip completed",
                    Message = $"Trip RT-{bid.Id:D6} has been verified and closed.",
                    Type = "Trip",
                    RelatedEntityType = "CargoBid",
                    RelatedEntityId = bid.Id,
                });
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            result = "approved";
        });

        if (result == "not-found") return NotFound();
        TempData[result == "approved" || result == "rejected" ? "Success" : "Error"] = result switch
        {
            "approved" => "Delivery and finances approved. Driver profile updated and trip closed.",
            "rejected" => "Delivery evidence rejected; the driver was notified.",
            "financial-check-failed" => "Trip cannot be closed until the deposit, fuel adjustments, and final balance are fully confirmed.",
            "evidence-missing" => "The stored delivery document is missing. Ask the driver to resubmit it before approval.",
            _ => "This delivery has already been reviewed.",
        };
        return RedirectToAction(nameof(Trips));
    }

    public async Task<IActionResult> Routes()
    {
        var (user, agent) = await GetAgentContextAsync();
        SetLayoutData(user, "routes");
        ViewData["Title"] = "Vehicle Routes";

        var vehicles = await _db.VehicleDetails
            .Where(v => v.VehicleOwnerId == agent.VehicleOwnerId &&
                        (!agent.CompanyId.HasValue || v.CompanyId == agent.CompanyId.Value))
            .Include(v => v.Company)
            .Include(v => v.Routes)
                .ThenInclude(route => route.CargoBids)
                    .ThenInclude(bid => bid.Cargo)
            .OrderBy(v => v.RegistrationNumber)
            .ToListAsync();

        var model = new AgentRoutesPageViewModel
        {
            Vehicles = vehicles.Select(vehicle =>
            {
                var currentRoute = vehicle.Routes
                    .OrderByDescending(route => route.UpdatedAt ?? route.CreatedAt)
                    .FirstOrDefault();

                return new AgentVehicleRouteViewModel
                {
                    VehicleId = vehicle.Id,
                    VehicleType = vehicle.VehicleType,
                    NumberPlate = vehicle.RegistrationNumber,
                    CompanyName = vehicle.Company.CompanyName,
                    FromLocation = currentRoute?.FromLocation,
                    FromPlaceId = currentRoute?.FromPlaceId,
                    FromLatitude = currentRoute?.FromLatitude,
                    FromLongitude = currentRoute?.FromLongitude,
                    ToLocation = currentRoute?.ToLocation,
                    ToPlaceId = currentRoute?.ToPlaceId,
                    ToLatitude = currentRoute?.ToLatitude,
                    ToLongitude = currentRoute?.ToLongitude,
                    DepartureDate = currentRoute?.TripStartsAt,
                    ReturnDate = currentRoute?.ReturnAvailabilityEndsAt ?? currentRoute?.ExpectedArrivalAt,
                    IsAwaitingDeposit = currentRoute?.CargoBids.Any(bid =>
                        bid.Status == "Accepted" && bid.PaymentStatus == "AwaitingDeposit") == true,
                    IsReservedForActiveCargo = currentRoute?.CargoBids.Any(bid =>
                        bid.Status == "Accepted" && bid.DepositPaidAt.HasValue && bid.Cargo.DeliveredAt == null) == true,
                };
            }).ToList(),
        };

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoute(AgentRouteUpdateViewModel model)
    {
        var (user, agent) = await GetAgentContextAsync();
        SetLayoutData(user, "routes");
        ViewData["Title"] = "Vehicle Routes";

        if (!ModelState.IsValid || model.DepartureDate == null || model.ReturnDate == null || model.ReturnDate < model.DepartureDate)
        {
            TempData["Error"] = "Enter both locations and valid dates. The return date cannot be before departure.";
            return RedirectToAction(nameof(Routes));
        }

        if (string.IsNullOrWhiteSpace(model.FromPlaceId) || model.FromLatitude == null || model.FromLongitude == null ||
            string.IsNullOrWhiteSpace(model.ToPlaceId) || model.ToLatitude == null || model.ToLongitude == null)
        {
            TempData["Error"] = "Select both locations from the map suggestions so cargo route matching can use their coordinates.";
            return RedirectToAction(nameof(Routes));
        }

        var vehicle = await _db.VehicleDetails.FirstOrDefaultAsync(v =>
            v.Id == model.VehicleId &&
            v.VehicleOwnerId == agent.VehicleOwnerId &&
            (!agent.CompanyId.HasValue || v.CompanyId == agent.CompanyId.Value));

        if (vehicle == null)
        {
            return NotFound();
        }

        var route = await _db.VehicleRoutes
            .Where(r => r.VehicleDetailId == vehicle.Id)
            .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
            .FirstOrDefaultAsync();

        var hasActiveBooking = route != null && await _db.CargoBids.AnyAsync(bid =>
            bid.VehicleRouteId == route.Id &&
            bid.Status == "Accepted" &&
            bid.PaymentStatus == "AwaitingDeposit");
        var hasActivePaidBooking = route != null && await _db.CargoBids.AnyAsync(bid =>
            bid.VehicleRouteId == route.Id &&
            bid.Status == "Accepted" &&
            bid.DepositPaidAt.HasValue &&
            bid.Cargo.DeliveredAt == null);
        if (hasActiveBooking && !hasActivePaidBooking)
        {
            TempData["Error"] = "This vehicle is held for a bid awaiting its deposit. Cancel the award or wait until payment is confirmed before changing its route.";
            return RedirectToAction(nameof(Routes));
        }

        if (route == null || (!hasActivePaidBooking &&
                              await _db.CargoBids.AnyAsync(bid => bid.VehicleRouteId == route.Id)))
        {
            route = new VehicleRoute
            {
                VehicleDetailId = vehicle.Id,
                CompanyAgentId = agent.Id,
            };
            _db.VehicleRoutes.Add(route);
        }

        var departureDate = model.DepartureDate.Value.Date;
        var returnDate = model.ReturnDate.Value.Date;
        route.CompanyAgentId = agent.Id;
        route.FromLocation = model.FromLocation.Trim();
        route.FromPlaceId = model.FromPlaceId;
        route.FromLatitude = model.FromLatitude;
        route.FromLongitude = model.FromLongitude;
        route.ToLocation = model.ToLocation.Trim();
        route.ToPlaceId = model.ToPlaceId;
        route.ToLatitude = model.ToLatitude;
        route.ToLongitude = model.ToLongitude;
        route.TripStartsAt = departureDate;
        route.ExpectedArrivalAt = returnDate;
        route.ReturnAvailabilityStartsAt = hasActivePaidBooking ? null : returnDate;
        route.ReturnAvailabilityEndsAt = hasActivePaidBooking ? null : returnDate;
        route.IsAwaitingReturn = !hasActivePaidBooking;
        route.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = $"Route updated for vehicle {vehicle.RegistrationNumber}.";
        return RedirectToAction(nameof(Routes));
    }

    private async Task<(UserAccount User, CompanyAgent Agent)> GetAgentContextAsync()
    {
        var user = await _userManager.GetUserAsync(User)
                   ?? throw new InvalidOperationException("User not found.");
        var agent = await _db.CompanyAgents
                         .FirstOrDefaultAsync(a => a.UserAccountId == user.Id && a.IsActive)
                     ?? throw new InvalidOperationException("Active agent profile not found.");
        return (user, agent);
    }

    private void SetLayoutData(UserAccount user, string activeNav)
    {
        ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}".Trim();
        ViewData["UserInitials"] = $"{user.FirstName?.FirstOrDefault()}{user.LastName?.FirstOrDefault()}".ToUpper();
        ViewData["DashRole"] = "CompanyAgent";
        ViewData["DashRoleLabel"] = "Company Agent";
        ViewData["ActiveNav"] = activeNav;
    }

    private static AgentVehicleOptionViewModel? CreateVehicleOption(VehicleRoute route, Cargo cargo)
    {
        var vehicle = route.VehicleDetail;
        if (!route.IsAwaitingReturn ||
            (vehicle.Capacity.HasValue && cargo.WeightKg > vehicle.Capacity.Value * 1000m) ||
            !route.FromLatitude.HasValue || !route.FromLongitude.HasValue ||
            !route.ToLatitude.HasValue || !route.ToLongitude.HasValue)
        {
            return null;
        }

        var hasCargoCoordinates =
            cargo.FromLatitude.HasValue && cargo.FromLongitude.HasValue &&
            cargo.ToLatitude.HasValue && cargo.ToLongitude.HasValue;
        var isOutboundMatch = true;
        var isReturnMatch = false;

        if (hasCargoCoordinates)
        {
            var outboundStartDistance = HaversineKm(
                cargo.FromLatitude!.Value, cargo.FromLongitude!.Value,
                route.FromLatitude.Value, route.FromLongitude.Value);
            var outboundEndDistance = HaversineKm(
                cargo.ToLatitude!.Value, cargo.ToLongitude!.Value,
                route.ToLatitude.Value, route.ToLongitude.Value);
            isOutboundMatch = outboundStartDistance <= 30 && outboundEndDistance <= 30;

            var returnStartDistance = HaversineKm(
                cargo.FromLatitude.Value, cargo.FromLongitude.Value,
                route.ToLatitude.Value, route.ToLongitude.Value);
            var returnEndDistance = HaversineKm(
                cargo.ToLatitude.Value, cargo.ToLongitude.Value,
                route.FromLatitude.Value, route.FromLongitude.Value);
            isReturnMatch = returnStartDistance <= 30 && returnEndDistance <= 30;

            if (!isOutboundMatch && !isReturnMatch)
            {
                return null;
            }
        }

        var pickupDate = (cargo.PickupDateTime ?? cargo.RequiredDepartureAt)?.Date;
        var availableFrom = isOutboundMatch
            ? route.TripStartsAt.Date
            : (route.ReturnAvailabilityStartsAt ?? route.ExpectedArrivalAt).Date;
        var availableUntil = isOutboundMatch
            ? route.ExpectedArrivalAt.Date
            : (route.ReturnAvailabilityEndsAt ?? route.ExpectedArrivalAt).Date;

        if (pickupDate.HasValue && (pickupDate.Value < availableFrom || pickupDate.Value > availableUntil))
        {
            return null;
        }

        return new AgentVehicleOptionViewModel
        {
            VehicleRouteId = route.Id,
            RegistrationNumber = vehicle.RegistrationNumber,
            VehicleType = vehicle.VehicleType,
            CompanyName = vehicle.Company.CompanyName,
            Route = $"{route.FromLocation} → {route.ToLocation}",
            Direction = !hasCargoCoordinates ? "Manual route check" : isOutboundMatch ? "Outbound" : "Return",
            RequiresManualRouteCheck = !hasCargoCoordinates,
            DepartureAt = isOutboundMatch ? route.TripStartsAt : route.ReturnAvailabilityStartsAt ?? route.ExpectedArrivalAt,
            AvailableUntil = isOutboundMatch ? route.ExpectedArrivalAt : route.ReturnAvailabilityEndsAt ?? route.ExpectedArrivalAt,
        };
    }

    private static double HaversineKm(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2)
    {
        const double earthRadiusKm = 6371;
        var lat1 = (double)latitude1 * Math.PI / 180;
        var lat2 = (double)latitude2 * Math.PI / 180;
        var deltaLat = ((double)latitude2 - (double)latitude1) * Math.PI / 180;
        var deltaLon = ((double)longitude2 - (double)longitude1) * Math.PI / 180;
        var a = Math.Pow(Math.Sin(deltaLat / 2), 2) +
                Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(deltaLon / 2), 2);
        return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
