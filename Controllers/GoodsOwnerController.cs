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

    // ── Stub actions (to be built next) ──────────────────────────────────────

    public IActionResult PostCargo()
    {
        ViewData["Title"] = "Post Cargo";
        return View("ComingSoon");
    }

    public IActionResult MyListings()
    {
        ViewData["Title"] = "My Listings";
        return View("ComingSoon");
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
