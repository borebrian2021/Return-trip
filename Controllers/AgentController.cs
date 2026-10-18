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

    public AgentController(SystemDBContext db, UserManager<UserAccount> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public IActionResult Dashboard() => RedirectToAction(nameof(Routes));

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
                    ReturnDate = currentRoute?.ReturnAvailabilityEndsAt,
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

        if (route == null)
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
        route.ReturnAvailabilityStartsAt = returnDate;
        route.ReturnAvailabilityEndsAt = returnDate;
        route.IsAwaitingReturn = true;
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
}
