using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Return_trip.Models;

namespace Return_trip.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SystemDBContext _db;
    private readonly UserManager<UserAccount> _userManager;
    private readonly SignInManager<UserAccount> _signInManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public HomeController(
        ILogger<HomeController> logger,
        SystemDBContext db,
        UserManager<UserAccount> userManager,
        SignInManager<UserAccount> signInManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _logger = logger;
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        return View("Landing");
    }

    public IActionResult Journey()
    {
        return View("Index");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RegisterGoodsOwner()
    {
        return View(new GoodsOwnerRegistrationViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterGoodsOwner(GoodsOwnerRegistrationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new UserAccount
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            PhoneNumber = model.Phone.Trim(),
            IsApproved = true
        };

        if (!await CreateIdentityAccountAsync(user, model.Password, AppRoles.GoodsOwner))
        {
            return View(model);
        }

        _db.GoodsOwners.Add(new GoodsOwner
        {
            UserAccountId = user.Id,
            Name = model.BusinessName.Trim(),
            Email = user.Email!,
            Phone = user.PhoneNumber!,
            Location = model.Location.Trim()
        });
        await _db.SaveChangesAsync();
        await _signInManager.SignInAsync(user, isPersistent: false);

        TempData["RegistrationSuccess"] = "Your goods owner account was created and you are signed in.";
        return RedirectToAction("Dashboard", "GoodsOwner");
    }

    [HttpGet]
    public IActionResult RegisterVehicleOwner()
    {
        return View(new VehicleOwnerRegistrationViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterVehicleOwner(VehicleOwnerRegistrationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new UserAccount
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            PhoneNumber = model.Phone.Trim(),
            IsApproved = true
        };

        if (!await CreateIdentityAccountAsync(user, model.Password, AppRoles.VehicleOwner))
        {
            return View(model);
        }

        _db.VehicleOwners.Add(new VehicleOwner
        {
            UserAccountId = user.Id,
            CompanyName = model.CompanyName.Trim(),
            BusinessRegistrationNumber = model.BusinessRegistrationNumber.Trim(),
            Email = user.Email!,
            Phone = user.PhoneNumber!,
            Location = model.Location.Trim(),
            IsApproved = true
        });
        await _db.SaveChangesAsync();
        await _signInManager.SignInAsync(user, isPersistent: false);

        TempData["RegistrationSuccess"] = "Your vehicle owner account was created and you are signed in. You can now register your drivers.";
        return RedirectToAction("Dashboard", "VehicleOwner");
    }

    [HttpGet]
    public IActionResult RegisterDriver()
    {
        return View(new DriverIdLookupViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterDriver(DriverIdLookupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var idNumber = model.IdNumber.Trim();
        var driver = await _db.Drivers.SingleOrDefaultAsync(d => d.IdNumber == idNumber);
        if (driver is null)
        {
            ModelState.AddModelError(nameof(model.IdNumber), "We could not verify that ID. Ask your vehicle owner to check that your driver profile has been entered.");
            return View(model);
        }

        if (driver.UserAccountId.HasValue)
        {
            ModelState.AddModelError(nameof(model.IdNumber), "An account has already been claimed for this driver profile. Use the sign-in page if this is your account.");
            return View(model);
        }

        return View("CompleteDriverRegistration", new DriverClaimViewModel
        {
            IdNumber = driver.IdNumber,
            ProfileName = driver.FullName,
            Email = driver.Email
        });
    }

    [HttpGet]
    public IActionResult RegisterAgent()
    {
        return View(new AgentEmailLookupViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAgent(AgentEmailLookupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var agent = await _db.CompanyAgents.SingleOrDefaultAsync(a => a.IdNumber == model.IdNumber.Trim());
        if (agent is null || agent.UserAccountId.HasValue || !agent.IsActive ||
            !string.Equals(agent.Email.Trim(), model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.IdNumber), "No unclaimed agent profile matches that ID number and email. Ask your vehicle owner to check your registered details.");
            return View(model);
        }

        return View("CompleteAgentRegistration", new AgentClaimViewModel
        {
            IdNumber = agent.IdNumber!,
            Email = agent.Email,
            ProfileName = agent.FullName
        });
    }

    public IActionResult RegisterSuccess(string role)
    {
        var model = new RegistrationSuccessViewModel
        {
            RoleLabel = role ?? "Account",
            Message = TempData["RegistrationSuccess"] as string ?? "Your registration request has been submitted successfully."
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email.Trim(), model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // role-based redirect — use the signed-in principal directly
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return LocalRedirect(model.ReturnUrl);

                if (await _userManager.IsInRoleAsync(user, AppRoles.GoodsOwner))
                    return RedirectToAction("Dashboard", "GoodsOwner");
                if (await _userManager.IsInRoleAsync(user, AppRoles.VehicleOwner))
                    return RedirectToAction("Dashboard", "VehicleOwner");
                if (await _userManager.IsInRoleAsync(user, AppRoles.Driver))
                    return RedirectToAction("Dashboard", "Driver");
                if (await _userManager.IsInRoleAsync(user, AppRoles.CompanyAgent))
                    return RedirectToAction("Dashboard", "Agent");
            }

            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty,
            result.IsLockedOut
                ? "This account is temporarily locked. Try again later."
                : "Email or password is incorrect.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.VehicleOwner)]
    public IActionResult AddDriver()
    {
        return View(new DriverProfileViewModel());
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.VehicleOwner)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDriver(DriverProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var vehicleOwner = await _db.VehicleOwners.SingleOrDefaultAsync(v => v.UserAccountId == user.Id);
        if (vehicleOwner is null)
        {
            return Forbid();
        }

        var idNumber = model.IdNumber.Trim();
        if (await _db.Drivers.AnyAsync(d => d.IdNumber == idNumber))
        {
            ModelState.AddModelError(nameof(model.IdNumber), "A driver profile with this ID is already registered.");
            return View(model);
        }

        var normalizedEmail = _userManager.NormalizeEmail(model.Email.Trim());
        if (await _db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
        {
            ModelState.AddModelError(nameof(model.Email), "This email address already belongs to an account.");
            return View(model);
        }

        _db.Drivers.Add(new Driver
        {
            VehicleOwnerId = vehicleOwner.Id,
            FullName = model.FullName.Trim(),
            IdNumber = idNumber,
            Email = model.Email.Trim(),
            PhoneNumber = model.PhoneNumber.Trim(),
            LicenseNumber = model.LicenseNumber.Trim(),
            IsVerified = false
        });
        await _db.SaveChangesAsync();

        TempData["RegistrationSuccess"] = "Driver details saved. The driver can now claim the profile with their ID number and set a sign-in password.";
        return RedirectToAction(nameof(RegisterSuccess), new { role = "Driver profile" });
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.VehicleOwner)]
    public IActionResult AddAgent()
    {
        return View(new AgentProfileViewModel());
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.VehicleOwner)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAgent(AgentProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var vehicleOwner = await _db.VehicleOwners.SingleOrDefaultAsync(v => v.UserAccountId == user.Id);
        if (vehicleOwner is null)
        {
            return Forbid();
        }

        var email = model.Email.Trim();
        var idNumber = model.IdNumber.Trim();
        if (await _db.CompanyAgents.AnyAsync(a => a.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "An agent profile with this email is already registered.");
            return View(model);
        }

        if (await _db.CompanyAgents.AnyAsync(a => a.IdNumber == idNumber))
        {
            ModelState.AddModelError(nameof(model.IdNumber), "An agent profile with this ID number is already registered.");
            return View(model);
        }

        var normalizedEmail = _userManager.NormalizeEmail(email);
        if (await _db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
        {
            ModelState.AddModelError(nameof(model.Email), "This email address already belongs to an account.");
            return View(model);
        }

        _db.CompanyAgents.Add(new CompanyAgent
        {
            VehicleOwnerId = vehicleOwner.Id,
            FullName = model.FullName.Trim(),
            IdNumber = idNumber,
            Email = email,
            PhoneNumber = model.PhoneNumber.Trim(),
            IsActive = true
        });
        await _db.SaveChangesAsync();

        TempData["RegistrationSuccess"] = "Agent details saved. The agent can claim the profile with their ID number and registered email, then set a password.";
        return RedirectToAction(nameof(RegisterSuccess), new { role = "Agent profile" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteDriverRegistration(DriverClaimViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("CompleteDriverRegistration", model);
        }

        var idNumber = model.IdNumber.Trim();
        var driver = await _db.Drivers.SingleOrDefaultAsync(d => d.IdNumber == idNumber);
        if (driver is null || driver.UserAccountId.HasValue)
        {
            ModelState.AddModelError(string.Empty, "This driver profile is unavailable. Check the ID or contact the vehicle owner.");
            return View("CompleteDriverRegistration", model);
        }

        if (!string.Equals(driver.Email.Trim(), model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Email), "Enter the email address recorded by your vehicle owner.");
            model.ProfileName = driver.FullName;
            return View("CompleteDriverRegistration", model);
        }

        var user = new UserAccount
        {
            UserName = driver.Email.Trim(),
            Email = driver.Email.Trim(),
            FirstName = driver.FullName.Trim(),
            PhoneNumber = driver.PhoneNumber.Trim(),
            IsApproved = true
        };

        if (!await CreateIdentityAccountAsync(user, model.Password, AppRoles.Driver))
        {
            model.ProfileName = driver.FullName;
            return View("CompleteDriverRegistration", model);
        }

        driver.UserAccountId = user.Id;
        driver.IsVerified = true;
        await _db.SaveChangesAsync();
        await _signInManager.SignInAsync(user, isPersistent: false);

        TempData["RegistrationSuccess"] = "Your existing driver profile is now connected to your sign-in account.";
        return RedirectToAction(nameof(RegisterSuccess), new { role = "Driver" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteAgentRegistration(AgentClaimViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("CompleteAgentRegistration", model);
        }

        var agent = await _db.CompanyAgents.SingleOrDefaultAsync(a =>
            a.IdNumber == model.IdNumber.Trim() && a.IsActive);
        if (agent != null && !string.Equals(agent.Email.Trim(), model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            agent = null;
        }
        if (agent is null || agent.UserAccountId.HasValue)
        {
            ModelState.AddModelError(string.Empty, "This agent profile is unavailable. Check your ID number and registered email or contact the vehicle owner.");
            return View("CompleteAgentRegistration", model);
        }

        var user = new UserAccount
        {
            UserName = agent.Email.Trim(),
            Email = agent.Email.Trim(),
            FirstName = agent.FullName.Trim(),
            PhoneNumber = agent.PhoneNumber.Trim(),
            IsApproved = true
        };

        if (!await CreateIdentityAccountAsync(user, model.Password, AppRoles.CompanyAgent))
        {
            model.ProfileName = agent.FullName;
            return View("CompleteAgentRegistration", model);
        }

        agent.UserAccountId = user.Id;
        await _db.SaveChangesAsync();
        await _signInManager.SignInAsync(user, isPersistent: false);

        TempData["RegistrationSuccess"] = "Your owner-created agent profile is now connected to your sign-in account.";
        return RedirectToAction(nameof(RegisterSuccess), new { role = "Company agent" });
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task<bool> CreateIdentityAccountAsync(UserAccount user, string password, string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>(role));
            if (!roleResult.Succeeded)
            {
                AddIdentityErrors(roleResult);
                return false;
            }
        }

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            return false;
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, role);
        if (addRoleResult.Succeeded)
        {
            return true;
        }

        AddIdentityErrors(addRoleResult);
        await _userManager.DeleteAsync(user);
        return false;
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
