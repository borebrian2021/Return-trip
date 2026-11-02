using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Return_trip.Models;

namespace Return_trip.Controllers;

[Authorize]
public sealed class NotificationsController : Controller
{
    private readonly SystemDBContext _db;
    private readonly UserManager<UserAccount> _userManager;

    public NotificationsController(SystemDBContext db, UserManager<UserAccount> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User)
            ?? throw new UnauthorizedAccessException("The signed-in account was not found.");
        var (role, roleLabel) = User.IsInRole(AppRoles.Driver)
            ? ("Driver", "Driver")
            : User.IsInRole(AppRoles.CompanyAgent)
                ? ("CompanyAgent", "Company Agent")
                : User.IsInRole(AppRoles.VehicleOwner)
                    ? ("VehicleOwner", "Vehicle Owner")
                    : ("GoodsOwner", "Goods Owner");
        ViewData["DashRole"] = role;
        ViewData["DashRoleLabel"] = roleLabel;
        ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}".Trim();
        ViewData["UserInitials"] = $"{user.FirstName.FirstOrDefault()}{user.LastName.FirstOrDefault()}".ToUpperInvariant();
        var notifications = await _db.Notifications
            .AsNoTracking()
            .Where(item => item.UserAccountId == user.Id)
            .OrderByDescending(item => item.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken);
        return View(notifications);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User)
            ?? throw new UnauthorizedAccessException("The signed-in account was not found.");
        var notification = await _db.Notifications.FirstOrDefaultAsync(
            item => item.Id == id && item.UserAccountId == user.Id,
            cancellationToken);
        if (notification == null) return NotFound();
        notification.IsRead = true;
        await _db.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
