using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Return_trip.Models;

namespace Return_trip.Controllers;

[Authorize(Roles = AppRoles.Driver)]
public sealed class DriverController : Controller
{
    private const long MaximumEvidenceBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedEvidenceTypes =
        ["image/jpeg", "image/png", "application/pdf"];

    private readonly SystemDBContext _db;
    private readonly UserManager<UserAccount> _userManager;
    private readonly IWebHostEnvironment _environment;

    public DriverController(
        SystemDBContext db,
        UserManager<UserAccount> userManager,
        IWebHostEnvironment environment)
    {
        _db = db;
        _userManager = userManager;
        _environment = environment;
    }

    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var driver = await GetDriverAsync(cancellationToken);
        ViewData["DashRole"] = "Driver";
        ViewData["DashRoleLabel"] = "Driver";
        ViewData["UserFullName"] = driver.FullName;
        ViewData["UserInitials"] = new string(driver.FullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(name => char.ToUpperInvariant(name[0]))
            .ToArray());
        ViewData["ActiveNav"] = "driver-trips";
        ViewData["Title"] = "My Trips";
        var bids = await GetAssignedTripsQuery(driver.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var trips = bids.Select(ToTripViewModel).ToList();
        return View(new DriverDashboardViewModel
        {
            DriverName = driver.FullName,
            PhoneNumber = driver.PhoneNumber,
            LicenseNumber = driver.LicenseNumber,
            AssignedVehicle = driver.VehicleDetail?.RegistrationNumber ?? "No vehicle assigned",
            DriverStatus = driver.IsVerified ? "Verified" : "Pending verification",
            LastCargoTransported = driver.LastCargoTransported,
            LastDeliveryAt = driver.LastDeliveryAt,
            CurrentActiveTrip = trips.FirstOrDefault(trip => trip.TripStatus != "Closed")?.Reference,
            TotalTrips = trips.Count,
            ActiveTrips = trips.Count(trip => trip.TripStatus != "Closed"),
            AwaitingPickup = trips.Count(trip => trip.TripStatus == "ReadyForPickup"),
            InTransit = trips.Count(trip => trip.TripStatus is "CargoPickedUp" or "InTransit"),
            AwaitingPayment = trips.Count(trip => trip.TripStatus is "ArrivedAtDestination" or "AwaitingFinalPayment"),
            CompletedTrips = trips.Count(trip => trip.TripStatus == "Closed"),
            Trips = trips,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AdvanceTrip(int id, string action, CancellationToken cancellationToken)
    {
        var driver = await GetDriverAsync(cancellationToken);
        var bid = await GetAssignedBidAsync(id, driver.Id, cancellationToken);
        if (bid == null) return NotFound();

        var next = (bid.TripStatus, action) switch
        {
            ("ReadyForPickup", "pickup") => "CargoPickedUp",
            ("CargoPickedUp", "depart") => "InTransit",
            ("InTransit", "arrive") => "ArrivedAtDestination",
            _ => null,
        };
        if (next == null || !bid.DepositPaidAt.HasValue || bid.PaymentStatus != "Paid")
        {
            TempData["Error"] = "This trip cannot be advanced from its current status.";
            return RedirectToAction(nameof(Dashboard));
        }

        var previousStatus = bid.TripStatus;
        bid.TripStatus = action == "arrive" && GetOutstandingBalance(bid) == 0m
            ? "PaymentConfirmed"
            : next;
        if (action == "depart")
        {
            bid.Cargo.DepartedAt = DateTime.UtcNow;
        }

        await AddActivityAsync(
            bid,
            driver.UserAccountId,
            bid.TripStatus,
            $"Trip status changed from {previousStatus} to {bid.TripStatus}.");

        if (action is "pickup" or "arrive")
        {
            var ownerAccountId = await _db.GoodsOwners
                .Where(owner => owner.Id == bid.Cargo.GoodsOwnerId)
                .Select(owner => owner.UserAccountId)
                .SingleAsync(cancellationToken);
            _db.Notifications.Add(new Notification
            {
                UserAccountId = ownerAccountId,
                Title = action == "arrive" ? "Cargo arrived" : "Cargo picked up",
                Message = action == "arrive" && GetOutstandingBalance(bid) > 0
                    ? $"Trip RT-{bid.Id:D6} has arrived. Please settle the outstanding balance before cargo handover."
                    : action == "arrive"
                        ? $"Trip RT-{bid.Id:D6} has arrived and the applicable balance is settled. Cargo handover may proceed."
                    : $"The driver picked up cargo for trip RT-{bid.Id:D6}.",
                Type = "Trip",
                RelatedEntityType = "CargoBid",
                RelatedEntityId = bid.Id,
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
        TempData["Success"] = action == "arrive"
            ? "Arrival recorded. The goods owner has been notified to settle the outstanding balance."
            : "Trip progress updated.";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitDeliveryEvidence(
        SubmitDeliveryEvidenceViewModel model,
        CancellationToken cancellationToken)
    {
        var driver = await GetDriverAsync(cancellationToken);
        var bid = await GetAssignedBidAsync(model.BidId, driver.Id, cancellationToken);
        if (bid == null) return NotFound();
        if (!ModelState.IsValid || model.SignedDocument == null)
        {
            TempData["Error"] = "Enter the receiver name and choose a valid signed delivery document.";
            return RedirectToAction(nameof(Dashboard));
        }

        var finalBalance = await GetOutstandingBalanceAsync(bid, cancellationToken);
        if (bid.TripStatus != "PaymentConfirmed" || finalBalance > 0 ||
            bid.FuelAdvances.Any(fuel => fuel.Status != "Reconciled"))
        {
            TempData["Error"] = "Delivery evidence is available only after the full applicable balance and fuel reconciliation are confirmed.";
            return RedirectToAction(nameof(Dashboard));
        }

        var file = model.SignedDocument;
        if (file.Length is <= 0 or > MaximumEvidenceBytes || !AllowedEvidenceTypes.Contains(file.ContentType))
        {
            TempData["Error"] = "Delivery evidence must be a non-empty JPEG, PNG, or PDF file no larger than 10 MB.";
            return RedirectToAction(nameof(Dashboard));
        }

        if (bid.DeliveryReceipt != null && bid.DeliveryReceipt.ConfirmationStatus != "Rejected")
        {
            TempData["Error"] = "Delivery evidence has already been submitted for this trip.";
            return RedirectToAction(nameof(Dashboard));
        }

        var extension = Path.GetExtension(Path.GetFileName(file.FileName));
        extension = extension.ToLowerInvariant();
        var expectedExtension = file.ContentType switch
        {
            "image/jpeg" => new[] { ".jpg", ".jpeg" },
            "image/png" => new[] { ".png" },
            "application/pdf" => new[] { ".pdf" },
            _ => [],
        };
        if (!expectedExtension.Contains(extension))
        {
            TempData["Error"] = "The selected file extension does not match an allowed delivery document type.";
            return RedirectToAction(nameof(Dashboard));
        }
        await using (var signatureStream = file.OpenReadStream())
        {
            var signature = new byte[8];
            var bytesRead = await signatureStream.ReadAsync(signature.AsMemory(), cancellationToken);
            var validSignature = file.ContentType switch
            {
                "image/jpeg" => bytesRead >= 3 && signature[0] == 0xFF && signature[1] == 0xD8 && signature[2] == 0xFF,
                "image/png" => bytesRead >= 8 && signature.SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
                "application/pdf" => bytesRead >= 5 && signature.AsSpan(0, 5).SequenceEqual("%PDF-"u8),
                _ => false,
            };
            if (!validSignature)
            {
                TempData["Error"] = "The uploaded file content does not match a valid JPEG, PNG, or PDF document.";
                return RedirectToAction(nameof(Dashboard));
            }
        }
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var storageDirectory = Path.Combine(_environment.ContentRootPath, "App_Data", "DeliveryEvidence");
        Directory.CreateDirectory(storageDirectory);
        var storagePath = Path.Combine(storageDirectory, fileName);
        var oldFilePath = bid.DeliveryReceipt == null
            ? null
            : Path.Combine(storageDirectory, Path.GetFileName(bid.DeliveryReceipt.ImageUrl));

        try
        {
            await using (var stream = new FileStream(storagePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var now = DateTime.UtcNow;
            bid.TripStatus = "AwaitingAgentVerification";
            if (bid.DeliveryReceipt is { ConfirmationStatus: "Rejected" } existingReceipt)
            {
                existingReceipt.ReceiverName = model.ReceiverName.Trim();
                existingReceipt.SignedAt = now;
                existingReceipt.ImageUrl = fileName;
                existingReceipt.ImageContentType = file.ContentType;
                existingReceipt.ImageFileName = Path.GetFileName(file.FileName);
                existingReceipt.ConfirmationStatus = "Pending";
                existingReceipt.ConfirmedByCompanyAgentId = null;
                existingReceipt.ConfirmedAt = null;
                existingReceipt.AgentComments = null;
                existingReceipt.UploadedAt = now;
            }
            else
            {
                bid.DeliveryReceipt = new DeliveryReceipt
                {
                    DriverId = driver.Id,
                    ReceiverName = model.ReceiverName.Trim(),
                    SignedAt = now,
                    ImageUrl = fileName,
                    ImageContentType = file.ContentType,
                    ImageFileName = Path.GetFileName(file.FileName),
                    ConfirmationStatus = "Pending",
                    UploadedAt = now,
                };
            }
            await AddActivityAsync(bid, driver.UserAccountId, "DeliveryEvidenceSubmitted", "Signed delivery evidence submitted for agent review.");

            var agentAccountIds = await _db.CompanyAgents
                .Where(agent => agent.Id == bid.CompanyAgentId && agent.UserAccountId.HasValue)
                .Select(agent => agent.UserAccountId!.Value)
                .ToListAsync(cancellationToken);
            foreach (var accountId in agentAccountIds)
            {
                _db.Notifications.Add(new Notification
                {
                    UserAccountId = accountId,
                    Title = "Delivery evidence submitted",
                    Message = $"Trip RT-{bid.Id:D6} is awaiting delivery and financial verification.",
                    Type = "Trip",
                    RelatedEntityType = "CargoBid",
                    RelatedEntityId = bid.Id,
                });
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            if (System.IO.File.Exists(storagePath))
            {
                System.IO.File.Delete(storagePath);
            }
            throw;
        }

        if (oldFilePath != null && System.IO.File.Exists(oldFilePath))
        {
            System.IO.File.Delete(oldFilePath);
        }

        TempData["Success"] = "Signed delivery evidence submitted for agent review.";
        return RedirectToAction(nameof(Dashboard));
    }

    private async Task<IActionResult> DownloadEvidenceCoreAsync(int id, CancellationToken cancellationToken)
    {
        var driver = await GetDriverAsync(cancellationToken);
        var receipt = await _db.DeliveryReceipts
            .AsNoTracking()
            .Include(item => item.CargoBid)
            .FirstOrDefaultAsync(item =>
                item.Id == id &&
                item.DriverId == driver.Id &&
                item.CargoBid.DriverId == driver.Id,
                cancellationToken);
        if (receipt == null) return NotFound();

        var path = Path.Combine(_environment.ContentRootPath, "App_Data", "DeliveryEvidence", receipt.ImageUrl);
        if (!System.IO.File.Exists(path)) return NotFound();
        return PhysicalFile(path, receipt.ImageContentType, receipt.ImageFileName ?? "delivery-evidence");
    }

    [HttpGet]
    public Task<IActionResult> DownloadEvidence(int id, CancellationToken cancellationToken) =>
        DownloadEvidenceCoreAsync(id, cancellationToken);

    private async Task<Driver> GetDriverAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User)
            ?? throw new UnauthorizedAccessException("The signed-in account was not found.");
        var driver = await _db.Drivers
            .Include(item => item.VehicleDetail)
            .SingleOrDefaultAsync(
            item => item.UserAccountId == user.Id,
            cancellationToken);
        return driver ?? throw new UnauthorizedAccessException("No driver profile is linked to this account.");
    }

    private IQueryable<CargoBid> GetAssignedTripsQuery(int driverId) =>
        _db.CargoBids
            .Where(bid => bid.DriverId == driverId &&
                          (bid.Status == "Accepted" || bid.Status == "Completed"))
            .Include(bid => bid.Driver)
            .Include(bid => bid.Cargo)
            .Include(bid => bid.VehicleRoute)
                .ThenInclude(route => route.VehicleDetail)
            .Include(bid => bid.Payments)
            .Include(bid => bid.FuelAdvances)
            .Include(bid => bid.Activities.OrderByDescending(activity => activity.CreatedAt).Take(20))
            .Include(bid => bid.DeliveryReceipt)
            .OrderByDescending(bid => bid.TripAssignedAt ?? bid.AcceptedAt);

    private Task<CargoBid?> GetAssignedBidAsync(int bidId, int driverId, CancellationToken cancellationToken) =>
        GetAssignedTripsQuery(driverId)
            .FirstOrDefaultAsync(bid => bid.Id == bidId, cancellationToken);

    private static decimal GetOutstandingBalance(CargoBid bid)
    {
        var paidDeposit = bid.PaymentStatus == "Paid" && bid.DepositPaidAt.HasValue
            ? bid.DepositAmount ?? 0m
            : 0m;
        var fuelCredit = bid.FuelAdvances
            .Where(fuel => fuel.Status == "Reconciled")
            .Sum(fuel => fuel.Amount);
        var finalPayments = bid.Payments
            .Where(payment => payment.Status == "Confirmed")
            .Sum(payment => payment.Amount);
        return Math.Max(0m, bid.BidAmount - paidDeposit - fuelCredit - finalPayments);
    }

    private Task<decimal> GetOutstandingBalanceAsync(CargoBid bid, CancellationToken cancellationToken) =>
        Task.FromResult(GetOutstandingBalance(bid));

    private static DriverTripViewModel ToTripViewModel(CargoBid bid)
    {
        var deposit = bid.PaymentStatus == "Paid" && bid.DepositPaidAt.HasValue
            ? bid.DepositAmount ?? 0m
            : 0m;
        var fuel = bid.FuelAdvances.Where(item => item.Status == "Reconciled").Sum(item => item.Amount);
        var finalPaid = bid.Payments.Where(item => item.Status == "Confirmed").Sum(item => item.Amount);
        var receiverUnlocked = bid.DepositPaidAt.HasValue && bid.PaymentStatus == "Paid";
        return new DriverTripViewModel
        {
            BidId = bid.Id,
            Reference = $"RT-{bid.Id:D6}",
            CargoTitle = bid.Cargo.Title,
            Description = bid.Cargo.Description,
            WeightKg = bid.Cargo.WeightKg,
            PickupLocation = bid.Cargo.FromLocation,
            Destination = bid.Cargo.ToLocation,
            VehicleRegistration = bid.VehicleRoute.VehicleDetail.RegistrationNumber,
            TripCost = bid.BidAmount,
            Deposit = deposit,
            FuelCredit = fuel,
            FinalBalance = Math.Max(0m, bid.BidAmount - deposit - fuel - finalPaid),
            PaymentStatus = bid.Payments.Any(payment => payment.Status == "PendingVerification")
                ? "Final payment pending verification"
                : bid.PaymentStatus,
            TripStatus = bid.TripStatus,
            AssignedAt = bid.TripAssignedAt ?? bid.AcceptedAt ?? bid.CreatedAt,
            ReceiverName = receiverUnlocked ? bid.Cargo.ReceiverName : null,
            ReceiverPhone = receiverUnlocked ? bid.Cargo.ReceiverPhone : null,
            Activities = bid.Activities.OrderByDescending(activity => activity.CreatedAt).ToList(),
        };
    }

    private Task AddActivityAsync(CargoBid bid, int? userAccountId, string activity, string details)
    {
        _db.TripActivities.Add(new TripActivity
        {
            CargoBid = bid,
            UserAccountId = userAccountId,
            Activity = activity,
            Details = details,
            CreatedAt = DateTime.UtcNow,
        });
        return Task.CompletedTask;
    }
}
