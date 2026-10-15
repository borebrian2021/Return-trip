using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Return_trip.Models;

namespace Return_trip.Controllers;

[Authorize(Roles = AppRoles.GoodsOwner)]
public class GoodsOwnerController : Controller
{
    private readonly SystemDBContext _db;
    private readonly UserManager<UserAccount> _userManager;

    public GoodsOwnerController(SystemDBContext db, UserManager<UserAccount> userManager)
    {
        _db = db;
        _userManager = userManager;
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
    public async Task<IActionResult> PostCargo()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "post");
        ViewData["Title"] = "Post Cargo";

        var commission = await _db.AdminCommissionSettings
                             .OrderByDescending(s => s.UpdatedAt)
                             .Select(s => s.Percentage)
                             .FirstOrDefaultAsync();

        return View(new PostCargoViewModel
        {
            CommissionPercent    = commission == 0 ? 10m : commission,
            RetentionPeriodDays  = 7,
            AdvancePaymentPercent = 70m,
            PricingMode          = "FixedTotal",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostCargo(PostCargoViewModel model)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "post");
        ViewData["Title"] = "Post Cargo";

        // cross-field validation
        var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(model);
        var extraErrors = model.Validate(ctx);
        foreach (var e in extraErrors)
            ModelState.AddModelError(e.MemberNames.FirstOrDefault() ?? string.Empty, e.ErrorMessage ?? "Invalid.");

        if (!ModelState.IsValid)
        {
            var commission = await _db.AdminCommissionSettings
                                 .OrderByDescending(s => s.UpdatedAt)
                                 .Select(s => s.Percentage)
                                 .FirstOrDefaultAsync();
            model.CommissionPercent = commission == 0 ? 10m : commission;
            return View(model);
        }

        var commission2 = await _db.AdminCommissionSettings
                              .OrderByDescending(s => s.UpdatedAt)
                              .Select(s => s.Percentage)
                              .FirstOrDefaultAsync();

        var cargo = new Cargo
        {
            GoodsOwnerId          = owner.Id,
            Title                 = model.Title.Trim(),
            Description           = model.Description.Trim(),
            PackageType           = model.PackageType.Trim(),
            WeightKg              = model.WeightKg,
            PricingMode           = model.PricingMode,
            TransportCost         = model.PricingMode == "FixedTotal"
                                        ? model.TransportCost!.Value
                                        : model.WeightKg * model.PricePerKg!.Value,
            PricePerKg            = model.PricingMode == "PerKg" ? model.PricePerKg : null,
            CommissionPercent     = commission2 == 0 ? 10m : commission2,
            RetentionPeriodDays   = model.RetentionPeriodDays,
            AdvancePaymentPercent = model.AdvancePaymentPercent,
            FromLocation          = model.FromLocation.Trim(),
            ToLocation            = model.ToLocation.Trim(),
            RequiredDepartureAt   = model.RequiredDepartureAt,
            DeliveryDeadlineAt    = model.DeliveryDeadlineAt,
            Status                = "Open",
            IsBidOpen             = true,
        };

        _db.Cargoes.Add(cargo);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Cargo listing \"{cargo.Title}\" posted successfully. Drivers can now bid on it.";
        return RedirectToAction(nameof(MyListings));
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

    public IActionResult Bids()
    {
        ViewData["Title"] = "Bids Received";
        return View("ComingSoon");
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
