using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Return_trip.Services;
using Return_trip.Models;

namespace Return_trip.Controllers;

[Authorize(Roles = AppRoles.GoodsOwner)]
public class GoodsOwnerController : Controller
{
    private const double RouteEndpointMatchRadiusKm = 30;

    private readonly SystemDBContext _db;
    private readonly UserManager<UserAccount> _userManager;
    private readonly IConfiguration _config;
    private readonly IDepositPaymentService _depositPaymentService;
    private readonly IWebHostEnvironment _environment;

    public GoodsOwnerController(
        SystemDBContext db,
        UserManager<UserAccount> userManager,
        IConfiguration config,
        IDepositPaymentService depositPaymentService,
        IWebHostEnvironment environment)
    {
        _db = db;
        _userManager = userManager;
        _config = config;
        _depositPaymentService = depositPaymentService;
        _environment = environment;
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<(UserAccount user, GoodsOwner owner)> GetContextAsync()
    {
        var user = await _userManager.GetUserAsync(User)
                   ?? throw new InvalidOperationException("User not found.");
        var owner = await _db.GoodsOwners
                        .FirstOrDefaultAsync(g => g.UserAccountId == user.Id)
                    ?? throw new InvalidOperationException("Goods owner profile not found.");
        return (user, owner);
    }

    private void SetLayoutData(UserAccount user, GoodsOwner owner, string activeNav)
    {
        ViewData["UserFullName"]  = $"{user.FirstName} {user.LastName}".Trim();
        ViewData["UserInitials"]  = $"{user.FirstName?.FirstOrDefault()}{user.LastName?.FirstOrDefault()}".ToUpper();
        ViewData["BusinessName"]  = owner.Name;
        ViewData["ActiveNav"]     = activeNav;
    }

    // ── Dashboard ─────────────────────────────────────────────────────────────

    public async Task<IActionResult> Dashboard()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "dashboard");
        ViewData["Title"] = "Dashboard";

        var cargoes = await _db.Cargoes
            .Where(c => c.GoodsOwnerId == owner.Id)
            .Include(c => c.Bids)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        // stat cards
        var vm = new GoodsOwnerDashboardViewModel
        {
            OwnerName    = $"{user.FirstName} {user.LastName}".Trim(),
            BusinessName = owner.Name,
            Location     = owner.Location,

            TotalListings  = cargoes.Count,
            OpenListings   = cargoes.Count(c => c.Status == "Open"),
            ActiveBids     = cargoes.Where(c => c.Status == "Open")
                                    .Sum(c => c.Bids.Count(b => b.Status == "Pending")),
            InTransit      = cargoes.Count(c => c.DepartedAt.HasValue && !c.DeliveredAt.HasValue),
            Delivered      = cargoes.Count(c => c.DeliveredAt.HasValue),
            TotalSpend     = cargoes.SelectMany(c => c.Bids)
                                    .Where(b => b.Status == "Accepted" || b.Status == "Completed")
                                    .Sum(b => b.BidAmount),
            PendingPriceDecisions = cargoes.SelectMany(c => c.Bids)
                                           .Count(b => b.PriceSuggestionStatus == "Pending"),
        };

        // recent listings (latest 8)
        vm.RecentListings = cargoes.Take(8).Select(c => new CargoRowViewModel
        {
            Id                  = c.Id,
            Title               = c.Title,
            Route               = $"{c.FromLocation} → {c.ToLocation}",
            WeightKg            = c.WeightKg,
            PackageType         = c.PackageType,
            Status              = c.Status,
            BidCount            = c.Bids.Count,
            CreatedAt           = c.CreatedAt,
            RequiredDepartureAt = c.RequiredDepartureAt,
        }).ToList();

        // recent bids across all cargo (latest 6)
        var recentBids = await _db.CargoBids
            .Where(b => b.Cargo.GoodsOwnerId == owner.Id)
            .Include(b => b.Cargo)
            .Include(b => b.CompanyAgent)
            .OrderByDescending(b => b.CreatedAt)
            .Take(6)
            .ToListAsync();

        vm.RecentBids = recentBids.Select(b => new BidRowViewModel
        {
            BidId                = b.Id,
            CargoId              = b.CargoId,
            CargoTitle           = b.Cargo.Title,
            Route                = $"{b.Cargo.FromLocation} → {b.Cargo.ToLocation}",
            AgentName            = b.CompanyAgent.FullName,
            BidAmount            = b.BidAmount,
            BidStatus            = b.Status,
            PriceSuggestionStatus = b.PriceSuggestionStatus,
            CreatedAt            = b.CreatedAt,
        }).ToList();

        // monthly cargo posted — last 6 months
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-5);
        var monthly = cargoes
            .Where(c => c.CreatedAt >= sixMonthsAgo)
            .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyStatViewModel
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                Count = g.Count()
            }).ToList();

        // fill any missing months with zero
        for (int i = 5; i >= 0; i--)
        {
            var d = DateTime.UtcNow.AddMonths(-i);
            var label = d.ToString("MMM");
            if (!monthly.Any(m => m.Month == label))
                monthly.Insert(5 - i, new MonthlyStatViewModel { Month = label, Count = 0 });
        }

        vm.MonthlyListings = monthly.TakeLast(6).ToList();

        return View(vm);
    }

    // ── Post Cargo ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> AvailableVehicles(
        decimal fromLatitude,
        decimal fromLongitude,
        decimal toLatitude,
        decimal toLongitude,
        DateTime? pickupDateTime)
    {
        if (fromLatitude is < -90 or > 90 || toLatitude is < -90 or > 90 ||
            fromLongitude is < -180 or > 180 || toLongitude is < -180 or > 180)
        {
            return BadRequest();
        }

        var routes = await _db.VehicleRoutes
            .Where(route => route.IsAwaitingReturn &&
                            route.FromLatitude.HasValue && route.FromLongitude.HasValue &&
                            route.ToLatitude.HasValue && route.ToLongitude.HasValue)
            .Include(route => route.VehicleDetail)
                .ThenInclude(vehicle => vehicle.Company)
            .ToListAsync();

        var requestedPickupDate = pickupDateTime?.Date;
        var matches = new List<(AvailableVehicleSuggestionViewModel Suggestion, double Score)>();

        foreach (var route in routes
                     .GroupBy(route => route.VehicleDetailId)
                     .Select(group => group.OrderByDescending(route => route.UpdatedAt ?? route.CreatedAt).First()))
        {
            var outboundStartDistance = HaversineKm(fromLatitude, fromLongitude, route.FromLatitude!.Value, route.FromLongitude!.Value);
            var outboundEndDistance = HaversineKm(toLatitude, toLongitude, route.ToLatitude!.Value, route.ToLongitude!.Value);
            var isOutboundMatch = outboundStartDistance <= RouteEndpointMatchRadiusKm &&
                                 outboundEndDistance <= RouteEndpointMatchRadiusKm;

            var returnStartDistance = HaversineKm(fromLatitude, fromLongitude, route.ToLatitude.Value, route.ToLongitude.Value);
            var returnEndDistance = HaversineKm(toLatitude, toLongitude, route.FromLatitude.Value, route.FromLongitude.Value);
            var isReturnMatch = returnStartDistance <= RouteEndpointMatchRadiusKm &&
                                returnEndDistance <= RouteEndpointMatchRadiusKm;

            var direction = "Outbound";
            var pickupDistance = outboundStartDistance;
            var deliveryDistance = outboundEndDistance;
            var availableFrom = route.TripStartsAt.Date;
            var availableUntil = route.ExpectedArrivalAt.Date;

            if (!isOutboundMatch && isReturnMatch)
            {
                direction = "Return";
                pickupDistance = returnStartDistance;
                deliveryDistance = returnEndDistance;
                availableFrom = route.ReturnAvailabilityStartsAt?.Date ?? route.ExpectedArrivalAt.Date;
                availableUntil = route.ReturnAvailabilityEndsAt?.Date ?? route.ExpectedArrivalAt.Date;
            }
            else if (!isOutboundMatch)
            {
                continue;
            }

            if (requestedPickupDate.HasValue &&
                (requestedPickupDate.Value < availableFrom || requestedPickupDate.Value > availableUntil))
            {
                continue;
            }

            var suggestion = new AvailableVehicleSuggestionViewModel
            {
                NumberPlate = route.VehicleDetail.RegistrationNumber,
                VehicleType = route.VehicleDetail.VehicleType,
                CompanyName = route.VehicleDetail.Company.CompanyName,
                FromLocation = route.FromLocation,
                ToLocation = route.ToLocation,
                Direction = direction,
                PickupDistanceKm = (decimal)Math.Round(pickupDistance, 1),
                DeliveryDistanceKm = (decimal)Math.Round(deliveryDistance, 1),
                DepartureDate = direction == "Return" ? availableFrom : route.TripStartsAt,
                ReturnDate = direction == "Return" ? availableUntil : route.ExpectedArrivalAt,
            };

            matches.Add((suggestion, pickupDistance + deliveryDistance));
        }

        return Json(matches
            .OrderBy(match => match.Score)
            .Take(8)
            .Select(match => match.Suggestion));
    }

    [HttpGet]
    public async Task<IActionResult> PostCargo()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "post");
        ViewData["Title"] = "Post Cargo";
        ViewData["GoogleMapsKey"] = _config["GoogleMaps:ApiKey"] ?? string.Empty;

        var commission = await _db.AdminCommissionSettings
                             .OrderByDescending(s => s.UpdatedAt)
                             .Select(s => s.Percentage)
                             .FirstOrDefaultAsync();

        return View(new PostCargoViewModel
        {
            WeightTonnes          = 0.2m,
            CommissionPercent     = commission == 0 ? 10m : commission,
            RetentionPeriodDays   = 7,
            AdvancePaymentPercent = 70m,
            PricingMode           = "FixedTotal",
            // pre-fill sender contact from profile
            SenderContactName     = $"{user.FirstName} {user.LastName}".Trim(),
            SenderPhone           = user.PhoneNumber ?? string.Empty,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostCargo(PostCargoViewModel model)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "post");
        ViewData["Title"] = "Post Cargo";
        ViewData["GoogleMapsKey"] = _config["GoogleMaps:ApiKey"] ?? string.Empty;

        var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(model);
        foreach (var e in model.Validate(ctx))
            ModelState.AddModelError(e.MemberNames.FirstOrDefault() ?? string.Empty, e.ErrorMessage ?? "Invalid.");

        if (!model.TermsAccepted)
            ModelState.AddModelError(nameof(model.TermsAccepted), "You must agree to the terms and conditions before posting cargo.");

        var weightKg = model.WeightTonnes * 1000m;

        var commission = await _db.AdminCommissionSettings
                             .OrderByDescending(s => s.UpdatedAt)
                             .Select(s => s.Percentage)
                             .FirstOrDefaultAsync();

        if (!ModelState.IsValid)
        {
            model.CommissionPercent = commission == 0 ? 10m : commission;
            return View(model);
        }

        var cargo = new Cargo
        {
            GoodsOwnerId          = owner.Id,
            Title                 = model.Title.Trim(),
            Description           = model.Description.Trim(),
            PackageType           = model.PackageType.Trim(),
            WeightKg              = weightKg,
            PricingMode           = model.PricingMode,
            TransportCost         = model.PricingMode == "FixedTotal"
                                        ? model.TransportCost!.Value
                                        : weightKg * model.PricePerKg!.Value,
            PricePerKg            = model.PricingMode == "PerKg" ? model.PricePerKg : null,
            CommissionPercent     = commission == 0 ? 10m : commission,
            RetentionPeriodDays   = model.RetentionPeriodDays,
            AdvancePaymentPercent = 70m,
            FromLocation          = model.FromLocation.Trim(),
            FromPlaceId           = model.FromPlaceId,
            FromLatitude          = model.FromLatitude,
            FromLongitude         = model.FromLongitude,
            ToLocation            = model.ToLocation.Trim(),
            ToPlaceId             = model.ToPlaceId,
            ToLatitude            = model.ToLatitude,
            ToLongitude           = model.ToLongitude,
            DistanceKm            = model.DistanceKm,
            PickupDateTime        = model.PickupDateTime,
            PickupDeadlineAt      = model.PickupDeadlineAt,
            DeliveryDeadlineAt    = model.DeliveryDeadlineAt,
            RequiredDepartureAt   = model.PickupDateTime,   // keep legacy field in sync
            SenderContactName     = model.SenderContactName.Trim(),
            SenderPhone           = model.SenderPhone.Trim(),
            ReceiverName          = model.ReceiverName.Trim(),
            ReceiverPhone         = model.ReceiverPhone.Trim(),
            ReceiverEmail         = model.ReceiverEmail?.Trim(),
            Status                = "Open",
            IsBidOpen             = true,
        };

        _db.Cargoes.Add(cargo);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Cargo listing \"{cargo.Title}\" posted successfully. Drivers can now bid on it.";
        return RedirectToAction(nameof(MyListings));
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

    // ── My Listings ───────────────────────────────────────────────────────────

    public async Task<IActionResult> MyListings(string? status = null)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "listings");
        ViewData["Title"] = "My Listings";

        var query = _db.Cargoes
            .Where(c => c.GoodsOwnerId == owner.Id)
            .Include(c => c.Bids)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(c => c.Status == status);

        var cargoes = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();

        var vm = new MyListingsViewModel
        {
            FilterStatus    = status,
            TotalCount      = await _db.Cargoes.CountAsync(c => c.GoodsOwnerId == owner.Id),
            OpenCount       = await _db.Cargoes.CountAsync(c => c.GoodsOwnerId == owner.Id && c.Status == "Open"),
            InTransitCount  = await _db.Cargoes.CountAsync(c => c.GoodsOwnerId == owner.Id
                                  && c.DepartedAt != null && c.DeliveredAt == null),
            DeliveredCount  = await _db.Cargoes.CountAsync(c => c.GoodsOwnerId == owner.Id
                                  && c.DeliveredAt != null),
            Listings = cargoes.Select(c => new CargoListingRow
            {
                Id                  = c.Id,
                Title               = c.Title,
                Route               = $"{c.FromLocation} → {c.ToLocation}",
                PackageType         = c.PackageType,
                WeightKg            = c.WeightKg,
                PricingMode         = c.PricingMode,
                TransportCost       = c.TransportCost,
                PricePerKg          = c.PricePerKg,
                Status              = c.Status,
                IsBidOpen           = c.IsBidOpen,
                BidCount            = c.Bids.Count,
                AcceptedBidCount    = c.Bids.Count(b => b.Status == "Accepted"),
                RequiredDepartureAt = c.RequiredDepartureAt,
                DeliveryDeadlineAt  = c.DeliveryDeadlineAt,
                CreatedAt           = c.CreatedAt,
            }).ToList()
        };

        return View(vm);
    }

    // ── Close / reopen bidding ────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBidding(int id)
    {
        var (_, owner) = await GetContextAsync();
        var cargo = await _db.Cargoes.FirstOrDefaultAsync(c => c.Id == id && c.GoodsOwnerId == owner.Id);
        if (cargo == null) return NotFound();

        if (cargo.Status is not ("Open" or "Closed"))
        {
            TempData["Error"] = "Bidding cannot be changed after a bid has been awarded.";
            return RedirectToAction(nameof(MyListings));
        }

        cargo.IsBidOpen = !cargo.IsBidOpen;
        if (!cargo.IsBidOpen && cargo.Status == "Open")
            cargo.Status = "Closed";
        else if (cargo.IsBidOpen && cargo.Status == "Closed")
        {
            cargo.Status      = "Open";
            cargo.ReopenedAt  = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = cargo.IsBidOpen
            ? $"\"{cargo.Title}\" is now open for bidding."
            : $"\"{cargo.Title}\" is now closed for bidding.";

        return RedirectToAction(nameof(MyListings));
    }

    // ── Delete listing (only if no accepted bids) ─────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCargo(int id)
    {
        var (_, owner) = await GetContextAsync();
        var cargo = await _db.Cargoes
            .Include(c => c.Bids)
            .FirstOrDefaultAsync(c => c.Id == id && c.GoodsOwnerId == owner.Id);

        if (cargo == null) return NotFound();

        if (cargo.Bids.Any(b => b.Status == "Accepted"))
        {
            TempData["Error"] = "Cannot delete a listing with an accepted bid.";
            return RedirectToAction(nameof(MyListings));
        }

        _db.Cargoes.Remove(cargo);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"\"{cargo.Title}\" has been deleted.";
        return RedirectToAction(nameof(MyListings));
    }

    public async Task<IActionResult> Bids()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "bids");
        ViewData["Title"] = "Bids Received";
        ViewData["AllowDepositSimulation"] = _environment.IsDevelopment();

        var bids = await _db.CargoBids
            .AsNoTracking()
            .Where(bid => bid.Cargo.GoodsOwnerId == owner.Id)
            .Include(bid => bid.Cargo)
            .Include(bid => bid.CompanyAgent)
            .Include(bid => bid.VehicleRoute)
                .ThenInclude(route => route.VehicleDetail)
                    .ThenInclude(vehicle => vehicle.Company)
            .Include(bid => bid.Payments)
            .Include(bid => bid.FuelAdvances)
            .OrderByDescending(bid => bid.CreatedAt)
            .ToListAsync();

        return View(new GoodsOwnerBidsViewModel
        {
            Bids = bids.Select(bid => new GoodsOwnerBidListItemViewModel
            {
                BidId = bid.Id,
                CargoTitle = bid.Cargo.Title,
                CargoRoute = $"{bid.Cargo.FromLocation} → {bid.Cargo.ToLocation}",
                PackageType = bid.Cargo.PackageType,
                WeightKg = bid.Cargo.WeightKg,
                AgentName = bid.CompanyAgent.FullName,
                CompanyName = bid.VehicleRoute.VehicleDetail.Company.CompanyName,
                VehicleRegistration = bid.VehicleRoute.VehicleDetail.RegistrationNumber,
                BidAmount = bid.BidAmount,
                CargoAskingPrice = bid.Cargo.TransportCost,
                SuggestedAmount = bid.SuggestedAmount,
                SuggestionComments = bid.SuggestionComments,
                PriceSuggestionStatus = bid.PriceSuggestionStatus,
                BidStatus = bid.Status,
                PaymentStatus = bid.PaymentStatus,
                DepositAmount = bid.DepositAmount,
                CreatedAt = bid.CreatedAt,
                TripStatus = bid.TripStatus,
                FinalBalance = GetFinalBalance(bid),
                FinalPaymentReference = bid.Payments.OrderByDescending(payment => payment.SubmittedAt)
                    .Select(payment => payment.Reference).FirstOrDefault(),
                FinalPaymentStatus = bid.Payments.OrderByDescending(payment => payment.SubmittedAt)
                    .Select(payment => payment.Status).FirstOrDefault(),
            }).ToList(),
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitFinalPayment(
        SubmitFinalPaymentViewModel model,
        CancellationToken cancellationToken)
    {
        var (user, owner) = await GetContextAsync();
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Enter a valid payment reference and amount.";
            return RedirectToAction(nameof(Bids));
        }

        var bid = await _db.CargoBids
            .Include(item => item.Cargo)
            .Include(item => item.Payments)
            .Include(item => item.FuelAdvances)
            .FirstOrDefaultAsync(item =>
                item.Id == model.BidId &&
                item.Cargo.GoodsOwnerId == owner.Id &&
                item.Status == "Accepted",
                cancellationToken);
        if (bid == null) return NotFound();

        var outstanding = GetFinalBalance(bid);
        if (bid.TripStatus is not ("ArrivedAtDestination" or "AwaitingFinalPayment") ||
            !bid.DepositPaidAt.HasValue ||
            bid.PaymentStatus != "Paid" ||
            bid.FuelAdvances.Any(fuel => fuel.Status != "Reconciled") ||
            bid.Payments.Any(payment => payment.Status == "PendingVerification") ||
            outstanding <= 0 ||
            decimal.Round(model.Amount, 2) != outstanding)
        {
            TempData["Error"] = "The final payment must match the verified outstanding balance after fuel reconciliation.";
            return RedirectToAction(nameof(Bids));
        }

        var reference = model.Reference.Trim();
        if (await _db.CargoPayments.AnyAsync(payment => payment.Reference == reference, cancellationToken))
        {
            TempData["Error"] = "That transaction reference has already been submitted.";
            return RedirectToAction(nameof(Bids));
        }

        var payment = new CargoPayment
        {
            CargoBidId = bid.Id,
            Amount = outstanding,
            Reference = reference,
            Status = "PendingVerification",
            SubmittedByUserAccountId = user.Id,
            SubmittedAt = DateTime.UtcNow,
        };
        bid.TripStatus = "AwaitingFinalPayment";
        _db.CargoPayments.Add(payment);
        _db.TripActivities.Add(new TripActivity
        {
            CargoBidId = bid.Id,
            UserAccountId = user.Id,
            Activity = "FinalPaymentSubmitted",
            Details = $"Manual payment reference {reference} submitted for KSh {outstanding:N2}.",
        });

        var agent = await _db.CompanyAgents
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == bid.CompanyAgentId, cancellationToken);
        if (agent?.UserAccountId is int agentAccountId)
        {
            _db.Notifications.Add(new Notification
            {
                UserAccountId = agentAccountId,
                Title = "Final payment submitted",
                Message = $"Manual payment for trip RT-{bid.Id:D6} is awaiting verification.",
                Type = "Trip",
                RelatedEntityType = "CargoBid",
                RelatedEntityId = bid.Id,
            });
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.GetBaseException() is SqlException { Number: 2601 or 2627 })
        {
            TempData["Error"] = "That transaction reference could not be recorded. Check that it has not already been used.";
            return RedirectToAction(nameof(Bids));
        }

        TempData["Success"] = "Payment reference submitted. Cargo handover remains locked until an agent confirms payment.";
        return RedirectToAction(nameof(Bids));
    }

    private static decimal GetFinalBalance(CargoBid bid)
    {
        var deposit = bid.PaymentStatus == "Paid" && bid.DepositPaidAt.HasValue
            ? bid.DepositAmount ?? 0m
            : 0m;
        var fuel = bid.FuelAdvances.Where(item => item.Status == "Reconciled").Sum(item => item.Amount);
        var finalPayments = bid.Payments.Where(item => item.Status == "Confirmed").Sum(item => item.Amount);
        return Math.Max(0m, bid.BidAmount - deposit - fuel - finalPayments);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReviewPriceSuggestion(int id, bool accept)
    {
        var (_, owner) = await GetContextAsync();
        var strategy = _db.Database.CreateExecutionStrategy();
        var outcome = string.Empty;

        await strategy.ExecuteAsync(async () =>
        {
            _db.ChangeTracker.Clear();
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var bid = await _db.CargoBids
                .Include(item => item.Cargo)
                .FirstOrDefaultAsync(item => item.Id == id && item.Cargo.GoodsOwnerId == owner.Id);

            if (bid == null)
            {
                outcome = "not-found";
                return;
            }

            if (bid.Status != "Pending" || bid.Cargo.Status != "Open" ||
                bid.PriceSuggestionStatus != "Pending" || !bid.SuggestedAmount.HasValue)
            {
                outcome = "not-available";
                return;
            }

            if (accept)
            {
                bid.BidAmount = bid.SuggestedAmount.Value;
                bid.PriceSuggestionStatus = "Accepted";
            }
            else
            {
                bid.PriceSuggestionStatus = "Rejected";
            }

            bid.PriceDecisionAt = DateTime.UtcNow;
            bid.PriceDecidedByGoodsOwnerId = owner.Id;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            outcome = accept ? "accepted" : "rejected";
        });

        if (outcome == "not-found") return NotFound();
        TempData[outcome == "accepted" || outcome == "rejected" ? "Success" : "Error"] = outcome switch
        {
            "accepted" => "Price suggestion accepted. You can now award or reject this bid.",
            "rejected" => "Price suggestion rejected. The agent's original bid remains available for your decision.",
            _ => "This price suggestion is no longer available for review.",
        };
        return RedirectToAction(nameof(Bids));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DecideBid(int id, bool accept)
    {
        var (_, owner) = await GetContextAsync();
        var strategy = _db.Database.CreateExecutionStrategy();
        string outcome = string.Empty;

        await strategy.ExecuteAsync(async () =>
        {
            _db.ChangeTracker.Clear();
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var bid = await _db.CargoBids
                .Include(item => item.Cargo)
                .ThenInclude(cargo => cargo.Bids)
                .Include(item => item.VehicleRoute)
                .FirstOrDefaultAsync(item => item.Id == id && item.Cargo.GoodsOwnerId == owner.Id);

            if (bid == null)
            {
                outcome = "not-found";
                return;
            }

            if (bid.Status != "Pending" || bid.Cargo.Status != "Open" ||
                bid.PriceSuggestionStatus == "Pending")
            {
                outcome = "not-available";
                return;
            }

            if (!accept)
            {
                bid.Status = "Rejected";
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                outcome = "rejected";
                return;
            }

            var latestRouteId = await _db.VehicleRoutes
                .Where(route => route.VehicleDetailId == bid.VehicleRoute.VehicleDetailId)
                .OrderByDescending(route => route.UpdatedAt ?? route.CreatedAt)
                .Select(route => route.Id)
                .FirstAsync();
            if (latestRouteId != bid.VehicleRouteId)
            {
                outcome = "vehicle-route-updated";
                return;
            }

            if (!bid.VehicleRoute.IsAwaitingReturn)
            {
                outcome = "vehicle-reserved";
                return;
            }

            bid.Status = "Accepted";
            bid.AcceptedAt = DateTime.UtcNow;
            bid.PaymentStatus = "AwaitingDeposit";
            bid.DepositAmount = decimal.Round(
                bid.BidAmount * 70m / 100m,
                2,
                MidpointRounding.AwayFromZero);
            bid.Cargo.Status = "AwaitingDeposit";
            bid.Cargo.IsBidOpen = false;
            bid.VehicleRoute.IsAwaitingReturn = false;

            foreach (var otherBid in bid.Cargo.Bids.Where(item => item.Id != bid.Id && item.Status == "Pending"))
            {
                otherBid.Status = "Rejected";
                if (otherBid.PriceSuggestionStatus == "Pending")
                {
                    otherBid.PriceSuggestionStatus = "Rejected";
                    otherBid.PriceDecisionAt = DateTime.UtcNow;
                    otherBid.PriceDecidedByGoodsOwnerId = owner.Id;
                }
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            outcome = "accepted";
        });

        if (outcome == "not-found") return NotFound();
        TempData[outcome == "accepted" || outcome == "rejected" ? "Success" : "Error"] = outcome switch
        {
            "accepted" => "Bid awarded. The vehicle is held while the 70% deposit is pending; its cargo route activates when payment is confirmed.",
            "rejected" => "Bid rejected.",
            "vehicle-reserved" => "This vehicle has already been held for another awarded bid. Select a different bid or vehicle.",
            "vehicle-route-updated" => "The agent has changed this vehicle's route since bidding. Ask them to submit an updated bid before awarding it.",
            _ => "This bid can no longer be accepted or rejected.",
        };
        return RedirectToAction(nameof(Bids));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelAward(int id)
    {
        var (_, owner) = await GetContextAsync();
        var strategy = _db.Database.CreateExecutionStrategy();
        var cancelled = false;
        var notFound = false;

        await strategy.ExecuteAsync(async () =>
        {
            _db.ChangeTracker.Clear();
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var bid = await _db.CargoBids
                .Include(item => item.Cargo)
                .Include(item => item.VehicleRoute)
                .FirstOrDefaultAsync(item =>
                    item.Id == id &&
                    item.Cargo.GoodsOwnerId == owner.Id);

            if (bid == null)
            {
                notFound = true;
                return;
            }

            if (bid.Status != "Accepted" ||
                bid.PaymentStatus != "AwaitingDeposit" ||
                bid.DepositPaidAt.HasValue ||
                bid.Cargo.Status != "AwaitingDeposit")
            {
                return;
            }

            bid.Status = "Rejected";
            bid.PaymentStatus = "Cancelled";
            bid.Cargo.Status = "Open";
            bid.Cargo.IsBidOpen = true;
            bid.VehicleRoute.IsAwaitingReturn = true;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            cancelled = true;
        });

        if (notFound) return NotFound();
        TempData[cancelled ? "Success" : "Error"] = cancelled
            ? "Award cancelled and cargo reopened. Agents may submit new bids."
            : "This award can no longer be cancelled or is already paid.";
        return RedirectToAction(nameof(Bids));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SimulateDepositPayment(int id, CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var (_, owner) = await GetContextAsync();
        var bid = await _db.CargoBids
            .Include(item => item.Cargo)
            .Include(item => item.VehicleRoute)
            .FirstOrDefaultAsync(item =>
                item.Id == id &&
                item.Cargo.GoodsOwnerId == owner.Id &&
                item.Status == "Accepted" &&
                item.Cargo.Status == "AwaitingDeposit" &&
                item.PaymentStatus == "AwaitingDeposit" &&
                !item.DepositPaidAt.HasValue,
                cancellationToken);
        if (bid == null) return NotFound();

        var payment = await _depositPaymentService.SimulateSuccessfulPaymentAsync(
            bid.DepositAmount ?? throw new InvalidOperationException("The awarded bid has no deposit amount."),
            cancellationToken);
        var strategy = _db.Database.CreateExecutionStrategy();
        var completed = false;

        await strategy.ExecuteAsync(async () =>
        {
            _db.ChangeTracker.Clear();
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
            var confirmedBid = await _db.CargoBids
                .Include(item => item.Cargo)
                .Include(item => item.VehicleRoute)
                .FirstOrDefaultAsync(item =>
                    item.Id == id &&
                    item.Cargo.GoodsOwnerId == owner.Id &&
                    item.Status == "Accepted" &&
                    item.Cargo.Status == "AwaitingDeposit" &&
                    item.PaymentStatus == "AwaitingDeposit" &&
                    !item.DepositPaidAt.HasValue,
                    cancellationToken);

            if (confirmedBid == null)
            {
                return;
            }

            var cargo = confirmedBid.Cargo;
            var route = confirmedBid.VehicleRoute;
            var pickupAt = cargo.PickupDateTime ?? cargo.RequiredDepartureAt ?? DateTime.UtcNow.Date;
            var arrivalAt = cargo.DeliveryDeadlineAt ?? route.ExpectedArrivalAt;
            if (arrivalAt < pickupAt)
            {
                arrivalAt = pickupAt;
            }

            route.FromLocation = cargo.FromLocation;
            route.FromPlaceId = cargo.FromPlaceId;
            route.FromLatitude = cargo.FromLatitude;
            route.FromLongitude = cargo.FromLongitude;
            route.ToLocation = cargo.ToLocation;
            route.ToPlaceId = cargo.ToPlaceId;
            route.ToLatitude = cargo.ToLatitude;
            route.ToLongitude = cargo.ToLongitude;
            route.TripStartsAt = pickupAt;
            route.ExpectedArrivalAt = arrivalAt;
            route.ReturnAvailabilityStartsAt = null;
            route.ReturnAvailabilityEndsAt = null;
            route.RoutePathJson = null;
            route.MapProvider = null;
            route.IsAwaitingReturn = false;
            route.UpdatedAt = payment.PaidAt;

            confirmedBid.PaymentStatus = "Paid";
            confirmedBid.DepositPaidAt = payment.PaidAt;
            confirmedBid.PaymentReference = payment.Reference;
            confirmedBid.EstimatedVehicleArrivalAt = arrivalAt;
            confirmedBid.TripStatus = "ReadyForPickup";
            cargo.Status = "Booked";

            _db.TripActivities.Add(new TripActivity
            {
                CargoBidId = confirmedBid.Id,
                UserAccountId = owner.UserAccountId,
                Activity = "DepositConfirmed",
                Details = "The initial deposit was confirmed; receiver contact access and pickup are enabled.",
                CreatedAt = payment.PaidAt,
            });
            if (confirmedBid.DriverId is int assignedDriverId)
            {
                var driverAccountId = await _db.Drivers
                    .Where(driver => driver.Id == assignedDriverId)
                    .Select(driver => driver.UserAccountId)
                    .SingleOrDefaultAsync(cancellationToken);
                if (driverAccountId.HasValue)
                {
                    _db.Notifications.Add(new Notification
                    {
                        UserAccountId = driverAccountId.Value,
                        Title = "Initial deposit confirmed",
                        Message = $"Trip RT-{confirmedBid.Id:D6} is ready for pickup. Receiver contact details are available in your trips dashboard.",
                        Type = "Trip",
                        RelatedEntityType = "CargoBid",
                        RelatedEntityId = confirmedBid.Id,
                    });
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            completed = true;
        });

        if (!completed)
        {
            TempData["Error"] = "The deposit could not be confirmed because this booking has changed. Refresh the bids page.";
            return RedirectToAction(nameof(Bids));
        }

        TempData["Success"] = "Simulated 70% deposit recorded. The vehicle is now reserved and its route is updated to the cargo journey.";
        return RedirectToAction(nameof(Bids));
    }

    public IActionResult InTransit()
    {
        ViewData["Title"] = "In Transit";
        return View("ComingSoon");
    }

    public IActionResult Banking()
    {
        ViewData["Title"] = "Bank Details";
        return View("ComingSoon");
    }

    public IActionResult Profile()
    {
        ViewData["Title"] = "My Profile";
        return View("ComingSoon");
    }
}
