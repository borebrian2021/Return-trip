using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Return_trip.Models;

namespace Return_trip.Controllers;

[Authorize(Roles = AppRoles.VehicleOwner)]
public class VehicleOwnerController : Controller
{
    private readonly SystemDBContext _db;
    private readonly UserManager<UserAccount> _userManager;
    private readonly IWebHostEnvironment _environment;

    public VehicleOwnerController(SystemDBContext db, UserManager<UserAccount> userManager, IWebHostEnvironment environment)
    {
        _db = db;
        _userManager = userManager;
        _environment = environment;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<(UserAccount user, VehicleOwner owner)> GetContextAsync()
    {
        var user = await _userManager.GetUserAsync(User)
                   ?? throw new InvalidOperationException("User not found.");
        var owner = await _db.VehicleOwners
                        .FirstOrDefaultAsync(v => v.UserAccountId == user.Id)
                    ?? throw new InvalidOperationException("Vehicle owner profile not found.");
        return (user, owner);
    }

    private void SetLayoutData(UserAccount user, VehicleOwner owner, string activeNav)
    {
        ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}".Trim();
        ViewData["UserInitials"] = $"{user.FirstName?.FirstOrDefault()}{user.LastName?.FirstOrDefault()}".ToUpper();
        ViewData["BusinessName"] = owner.CompanyName;
        ViewData["ActiveNav"]    = activeNav;
        ViewData["DashRole"]     = "VehicleOwner";
    }

    // ── Dashboard ─────────────────────────────────────────────────────────────

    public async Task<IActionResult> Dashboard()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "dashboard");
        ViewData["Title"] = "Dashboard";

        var drivers = await _db.Drivers
            .Where(d => d.VehicleOwnerId == owner.Id)
            .Include(d => d.VehicleDetail)
            .ToListAsync();

        var agents = await _db.CompanyAgents
            .Where(a => a.VehicleOwnerId == owner.Id)
            .Include(a => a.Company)
            .ToListAsync();

        var vehicles = await _db.VehicleDetails
            .Where(v => v.VehicleOwnerId == owner.Id)
            .Include(v => v.Company)
            .Include(v => v.Routes)
            .ToListAsync();

        var companies = await _db.CompanyRegistrations
            .Where(c => c.VehicleOwnerId == owner.Id)
            .CountAsync();

        // bids placed by this owner's agents
        var bids = await _db.CargoBids
            .Where(b => b.CompanyAgent.VehicleOwnerId == owner.Id)
            .Include(b => b.Cargo)
            .Include(b => b.CompanyAgent)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var vm = new VehicleOwnerDashboardViewModel
        {
            OwnerName         = $"{user.FirstName} {user.LastName}".Trim(),
            CompanyName       = owner.CompanyName,
            Location          = owner.Location,
            TotalDrivers      = drivers.Count,
            VerifiedDrivers   = drivers.Count(d => d.IsVerified),
            TotalAgents       = agents.Count,
            ActiveAgents      = agents.Count(a => a.IsActive),
            TotalVehicles     = vehicles.Count,
            ApprovedVehicles  = vehicles.Count(v => v.IsApproved),
            TotalCompanies    = companies,
            ActiveBids        = bids.Count(b => b.Status == "Pending"),
            InTransitCargo    = bids.Count(b => b.Status == "Accepted" && b.Cargo.DepartedAt.HasValue && !b.Cargo.DeliveredAt.HasValue),
            DeliveredThisMonth = bids.Count(b => b.Status == "Completed" && b.CompletedAt >= startOfMonth),
            EarningsThisMonth = bids
                .Where(b => b.Status == "Completed" && b.CompletedAt >= startOfMonth)
                .Sum(b => b.BidAmount),
            CurrentRoutes = vehicles
                .Select(BuildOwnerVehicleRouteRow)
                .OrderByDescending(v => v.UpdatedAt ?? DateTime.MinValue)
                .Take(6)
                .ToList(),
        };

        vm.RecentDrivers = drivers.OrderByDescending(d => d.CreatedAt).Take(8)
            .Select(d => new DriverRowViewModel
            {
                Id            = d.Id,
                FullName      = d.FullName,
                IdNumber      = d.IdNumber,
                PhoneNumber   = d.PhoneNumber,
                LicenseNumber = d.LicenseNumber,
                VehicleNumberPlate = d.VehicleDetail?.RegistrationNumber,
                IsVerified    = d.IsVerified,
                HasAccount    = d.UserAccountId.HasValue,
                CreatedAt     = d.CreatedAt,
            }).ToList();

        vm.RecentAgents = agents.OrderByDescending(a => a.Id).Take(6)
            .Select(a => new AgentRowViewModel
            {
                Id          = a.Id,
                FullName    = a.FullName,
                Email       = a.Email,
                PhoneNumber = a.PhoneNumber,
                IsActive    = a.IsActive,
                HasAccount  = a.UserAccountId.HasValue,
                CompanyName = a.Company?.CompanyName,
            }).ToList();

        vm.RecentBids = bids.Take(6)
            .Select(b => new VoBidRowViewModel
            {
                BidId      = b.Id,
                CargoTitle = b.Cargo.Title,
                Route      = $"{b.Cargo.FromLocation} → {b.Cargo.ToLocation}",
                AgentName  = b.CompanyAgent.FullName,
                BidAmount  = b.BidAmount,
                Status     = b.Status,
                CreatedAt  = b.CreatedAt,
            }).ToList();

        // monthly bids last 6 months
        var sixAgo = now.AddMonths(-5);
        var monthly = bids
            .Where(b => b.CreatedAt >= sixAgo)
            .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyStatViewModel
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                Count = g.Count(),
            }).ToList();

        for (int i = 5; i >= 0; i--)
        {
            var d = now.AddMonths(-i);
            var label = d.ToString("MMM");
            if (!monthly.Any(m => m.Month == label))
                monthly.Insert(5 - i, new MonthlyStatViewModel { Month = label, Count = 0 });
        }
        vm.MonthlyBids = monthly.TakeLast(6).ToList();

        return View(vm);
    }

    // ── Drivers list ──────────────────────────────────────────────────────────

    public async Task<IActionResult> Drivers(string? filter = null)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "drivers");
        ViewData["Title"]  = "Drivers";
        ViewData["Filter"] = filter;

        var query = _db.Drivers.Where(d => d.VehicleOwnerId == owner.Id).AsQueryable();
        if (filter == "verified")   query = query.Where(d => d.IsVerified);
        if (filter == "unverified") query = query.Where(d => !d.IsVerified);
        if (filter == "claimed")    query = query.Where(d => d.UserAccountId != null);
        if (filter == "unclaimed")  query = query.Where(d => d.UserAccountId == null);

        var drivers = await query
            .Include(d => d.VehicleDetail)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

        var rows = drivers.Select(d => new DriverRowViewModel
        {
            Id            = d.Id,
            FullName      = d.FullName,
            IdNumber      = d.IdNumber,
            PhoneNumber   = d.PhoneNumber,
            LicenseNumber = d.LicenseNumber,
            VehicleNumberPlate = d.VehicleDetail?.RegistrationNumber,
            IsVerified    = d.IsVerified,
            HasAccount    = d.UserAccountId.HasValue,
            CreatedAt     = d.CreatedAt,
        }).ToList();

        return View(rows);
    }

    // ── Add Driver ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> AddDriver()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "drivers");
        ViewData["Title"] = "Add Driver";

        await LoadDriverVehicleOptionsAsync(owner.Id);
        if (!await _db.VehicleDetails.AnyAsync(v => v.VehicleOwnerId == owner.Id))
        {
            TempData["Warning"] = "Register a vehicle before assigning it to a driver.";
            return RedirectToAction(nameof(AddVehicle));
        }

        return View(new DriverProfileViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDriver(DriverProfileViewModel model)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "drivers");
        ViewData["Title"] = "Add Driver";

        await LoadDriverVehicleOptionsAsync(owner.Id);
        if (!await _db.VehicleDetails.AnyAsync(v => v.VehicleOwnerId == owner.Id))
        {
            TempData["Warning"] = "Register a vehicle before assigning it to a driver.";
            return RedirectToAction(nameof(AddVehicle));
        }

        if (!ModelState.IsValid) return View(model);

        if (!await _db.VehicleDetails.AnyAsync(v => v.Id == model.VehicleDetailId && v.VehicleOwnerId == owner.Id))
        {
            ModelState.AddModelError(nameof(model.VehicleDetailId), "Select a vehicle registered under your account.");
            return View(model);
        }

        if (await _db.Drivers.AnyAsync(d => d.IdNumber == model.IdNumber.Trim()))
        {
            ModelState.AddModelError(nameof(model.IdNumber), "A driver with this ID is already registered.");
            return View(model);
        }

        var normalizedEmail = _userManager.NormalizeEmail(model.Email.Trim());
        if (await _db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
        {
            ModelState.AddModelError(nameof(model.Email), "This email already belongs to an account.");
            return View(model);
        }

        _db.Drivers.Add(new Driver
        {
            VehicleOwnerId = owner.Id,
            VehicleDetailId = model.VehicleDetailId,
            FullName       = model.FullName.Trim(),
            IdNumber       = model.IdNumber.Trim(),
            Email          = model.Email.Trim(),
            PhoneNumber    = model.PhoneNumber.Trim(),
            LicenseNumber  = model.LicenseNumber.Trim(),
            IsVerified     = false,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Driver \"{model.FullName}\" added. They can now claim the profile using their ID number.";
        return RedirectToAction(nameof(Drivers));
    }

    private async Task LoadDriverVehicleOptionsAsync(int ownerId)
    {
        ViewBag.Vehicles = await _db.VehicleDetails
            .Where(v => v.VehicleOwnerId == ownerId)
            .OrderBy(v => v.RegistrationNumber)
            .Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = $"{v.RegistrationNumber} - {v.VehicleType}",
            })
            .ToListAsync();
    }

    // ── Agents list ───────────────────────────────────────────────────────────

    public async Task<IActionResult> Agents(string? filter = null)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "agents");
        ViewData["Title"]  = "Agents";
        ViewData["Filter"] = filter;

        var query = _db.CompanyAgents
            .Where(a => a.VehicleOwnerId == owner.Id)
            .Include(a => a.Company)
            .AsQueryable();

        if (filter == "active")   query = query.Where(a => a.IsActive);
        if (filter == "inactive") query = query.Where(a => !a.IsActive);
        if (filter == "claimed")  query = query.Where(a => a.UserAccountId != null);
        if (filter == "unclaimed")query = query.Where(a => a.UserAccountId == null);

        var agents = await query.OrderByDescending(a => a.Id).ToListAsync();

        var rows = agents.Select(a => new AgentRowViewModel
        {
            Id          = a.Id,
            FullName    = a.FullName,
            IdNumber    = a.IdNumber ?? string.Empty,
            Email       = a.Email,
            PhoneNumber = a.PhoneNumber,
            IsActive    = a.IsActive,
            HasAccount  = a.UserAccountId.HasValue,
            CompanyName = a.Company?.CompanyName,
        }).ToList();

        return View(rows);
    }

    // ── Add Agent ─────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> AddAgent()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "agents");
        ViewData["Title"] = "Add Agent";
        return View(new AgentProfileViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAgent(AgentProfileViewModel model)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "agents");
        ViewData["Title"] = "Add Agent";

        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim();
        var idNumber = model.IdNumber.Trim();
        if (await _db.CompanyAgents.AnyAsync(a => a.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "An agent with this email is already registered.");
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
            ModelState.AddModelError(nameof(model.Email), "This email already belongs to an account.");
            return View(model);
        }

        _db.CompanyAgents.Add(new CompanyAgent
        {
            VehicleOwnerId = owner.Id,
            FullName       = model.FullName.Trim(),
            IdNumber       = idNumber,
            Email          = email,
            PhoneNumber    = model.PhoneNumber.Trim(),
            IsActive       = true,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Agent \"{model.FullName}\" added. They can claim the profile using their ID number and registered email.";
        return RedirectToAction(nameof(Agents));
    }

    // ── Toggle agent active ───────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAgent(int id)
    {
        var (_, owner) = await GetContextAsync();
        var agent = await _db.CompanyAgents
            .FirstOrDefaultAsync(a => a.Id == id && a.VehicleOwnerId == owner.Id);
        if (agent == null) return NotFound();

        agent.IsActive = !agent.IsActive;
        await _db.SaveChangesAsync();
        TempData["Success"] = $"\"{agent.FullName}\" is now {(agent.IsActive ? "active" : "inactive")}.";
        return RedirectToAction(nameof(Agents));
    }

    // ── Vehicles list ─────────────────────────────────────────────────────────

    public async Task<IActionResult> Vehicles()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "vehicles");
        ViewData["Title"] = "Vehicles";

        var vehicles = await _db.VehicleDetails
            .Where(v => v.VehicleOwnerId == owner.Id)
            .Include(v => v.Company)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return View(vehicles);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadVehicleDocument(int id, string document)
    {
        var (_, owner) = await GetContextAsync();
        var vehicle = await _db.VehicleDetails
            .FirstOrDefaultAsync(v => v.Id == id && v.VehicleOwnerId == owner.Id);
        if (vehicle == null)
        {
            return NotFound();
        }

        var (key, contentType) = document switch
        {
            "insurance" => (vehicle.InsuranceDocumentUrl, vehicle.InsuranceDocumentType),
            "inspection" => (vehicle.InspectionReportUrl, vehicle.InspectionReportType),
            _ => (null, null),
        };

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(contentType) || Path.GetFileName(key) != key)
        {
            return NotFound();
        }

        var path = Path.Combine(_environment.ContentRootPath, "App_Data", "vehicle-documents", key);
        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        return PhysicalFile(path, contentType, $"vehicle-{id}-{document}{Path.GetExtension(key)}");
    }

    // ── Companies ───────────────────────────────────────────────────────────

    public async Task<IActionResult> Companies()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "companies");
        ViewData["Title"] = "Companies";

        var companies = await _db.CompanyRegistrations
            .Where(c => c.VehicleOwnerId == owner.Id)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return View(companies);
    }

    [HttpGet]
    public async Task<IActionResult> AddCompany()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "companies");
        ViewData["Title"] = "Register Company";
        return View(new CompanyRegistrationViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(11_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10_500_000)]
    public async Task<IActionResult> AddCompany(CompanyRegistrationViewModel model)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "companies");
        ViewData["Title"] = "Register Company";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var certificate = model.CertificateOfRegistration!;
        var certificateExtension = Path.GetExtension(certificate.FileName).ToLowerInvariant();
        var allowedCertificates = new Dictionary<string, string>
        {
            [".pdf"] = "application/pdf",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
        };

        if (certificate.Length == 0 || certificate.Length > 10 * 1024 * 1024 ||
            !allowedCertificates.TryGetValue(certificateExtension, out var certificateContentType) ||
            !string.Equals(certificate.ContentType, certificateContentType, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.CertificateOfRegistration), "Upload a PDF, JPG, or PNG certificate no larger than 10 MB.");
            return View(model);
        }

        var companyName = model.CompanyName.Trim();
        var registrationNumber = model.RegistrationNumber.Trim();

        if (await _db.CompanyRegistrations.AnyAsync(c => c.VehicleOwnerId == owner.Id && c.CompanyName == companyName))
        {
            ModelState.AddModelError(nameof(model.CompanyName), "You have already registered a company with this name.");
            return View(model);
        }

        if (await _db.CompanyRegistrations.AnyAsync(c => c.VehicleOwnerId == owner.Id && c.RegistrationNumber == registrationNumber))
        {
            ModelState.AddModelError(nameof(model.RegistrationNumber), "This registration number is already linked to another company on your account.");
            return View(model);
        }

        var certificateKey = $"{Guid.NewGuid():N}{certificateExtension}";
        var certificateDirectory = Path.Combine(_environment.ContentRootPath, "App_Data", "company-certificates");
        Directory.CreateDirectory(certificateDirectory);
        var certificatePath = Path.Combine(certificateDirectory, certificateKey);

        await using (var fileStream = new FileStream(certificatePath, FileMode.CreateNew, FileAccess.Write))
        {
            await certificate.CopyToAsync(fileStream);
        }

        _db.CompanyRegistrations.Add(new CompanyRegistration
        {
            VehicleOwnerId = owner.Id,
            CompanyName = companyName,
            RegistrationNumber = registrationNumber,
            BusinessSector = model.BusinessSector.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone.Trim(),
            Location = model.Location.Trim(),
            CertificateOfRegistrationUrl = certificateKey,
            CertificateOfRegistrationType = certificateContentType,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow,
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            System.IO.File.Delete(certificatePath);
            throw;
        }

        TempData["Success"] = $"Company \"{companyName}\" has been registered.";
        return RedirectToAction(nameof(Companies));
    }

    [HttpGet]
    public async Task<IActionResult> DownloadCompanyCertificate(int id)
    {
        var (_, owner) = await GetContextAsync();
        var company = await _db.CompanyRegistrations
            .FirstOrDefaultAsync(c => c.Id == id && c.VehicleOwnerId == owner.Id);

        if (company == null || string.IsNullOrWhiteSpace(company.CertificateOfRegistrationUrl))
        {
            return NotFound();
        }

        var certificateKey = company.CertificateOfRegistrationUrl;
        if (Path.GetFileName(certificateKey) != certificateKey)
        {
            return NotFound();
        }

        var certificatePath = Path.Combine(_environment.ContentRootPath, "App_Data", "company-certificates", certificateKey);
        if (!System.IO.File.Exists(certificatePath))
        {
            return NotFound();
        }

        var extension = Path.GetExtension(certificateKey);
        return PhysicalFile(certificatePath, company.CertificateOfRegistrationType, $"company-registration-{company.Id}{extension}");
    }

    [HttpGet]
    public async Task<IActionResult> AddVehicle()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "vehicles");
        ViewData["Title"] = "Add Vehicle";

        var companies = await _db.CompanyRegistrations
            .Where(c => c.VehicleOwnerId == owner.Id)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();

        if (!companies.Any())
        {
            TempData["Warning"] = "Register a company before adding vehicles to your fleet.";
            return RedirectToAction(nameof(AddCompany));
        }

        ViewBag.Companies = companies
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CompanyName })
            .ToList();

        return View(new VehicleDetailViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(21_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 20_500_000)]
    public async Task<IActionResult> AddVehicle(VehicleDetailViewModel model)
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "vehicles");
        ViewData["Title"] = "Add Vehicle";

        var companies = await _db.CompanyRegistrations
            .Where(c => c.VehicleOwnerId == owner.Id)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();

        ViewBag.Companies = companies
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CompanyName })
            .ToList();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var insuranceValid = await IsValidVehicleDocumentAsync(model.InsuranceDocument);
        var inspectionValid = await IsValidVehicleDocumentAsync(model.InspectionReport);
        if (!insuranceValid)
        {
            ModelState.AddModelError(nameof(model.InsuranceDocument), "Upload a PDF, JPG, or PNG insurance document no larger than 10 MB.");
        }
        if (!inspectionValid)
        {
            ModelState.AddModelError(nameof(model.InspectionReport), "Upload a PDF, JPG, or PNG inspection report no larger than 10 MB.");
        }
        if (!insuranceValid || !inspectionValid)
        {
            return View(model);
        }

        var companyExists = companies.Any(c => c.Id == model.CompanyId);
        if (!companyExists)
        {
            ModelState.AddModelError(nameof(model.CompanyId), "Please select a valid company from your list.");
            return View(model);
        }

        var registrationNumber = model.RegistrationNumber.Trim();
        if (await _db.VehicleDetails.AnyAsync(v => v.VehicleOwnerId == owner.Id && v.RegistrationNumber == registrationNumber))
        {
            ModelState.AddModelError(nameof(model.RegistrationNumber), "A vehicle with this registration number is already on your account.");
            return View(model);
        }

        var insuranceFile = await SaveVehicleDocumentAsync(model.InsuranceDocument!);
        var inspectionFile = await SaveVehicleDocumentAsync(model.InspectionReport!);
        var vehicle = new VehicleDetail
        {
            VehicleOwnerId = owner.Id,
            CompanyId = model.CompanyId,
            VehicleType = model.VehicleType.Trim(),
            RegistrationNumber = registrationNumber,
            InsuranceDocumentUrl = insuranceFile.Key,
            InsuranceDocumentType = insuranceFile.ContentType,
            InsuranceExpiryDate = model.InsuranceExpiryDate!.Value,
            InspectionReportUrl = inspectionFile.Key,
            InspectionReportType = inspectionFile.ContentType,
            InspectionExpiryDate = model.InspectionExpiryDate!.Value,
            Capacity = model.Capacity!.Value,
            LastCargoTransported = model.LastCargoTransported.Trim(),
            IsApproved = false,
            CreatedAt = DateTime.UtcNow,
        };

        _db.VehicleDetails.Add(vehicle);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            System.IO.File.Delete(insuranceFile.Path);
            System.IO.File.Delete(inspectionFile.Path);
            throw;
        }

        TempData["Success"] = $"Vehicle \"{registrationNumber}\" has been added for your selected company.";
        return RedirectToAction(nameof(Vehicles));
    }

    private static async Task<bool> IsValidVehicleDocumentAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0 || file.Length > 10 * 1024 * 1024)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var signature = new byte[8];
        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(signature);

        return extension switch
        {
            ".pdf" => bytesRead >= 5 && signature.AsSpan(0, 5).SequenceEqual("%PDF-"u8),
            ".jpg" or ".jpeg" => bytesRead >= 3 && signature[0] == 0xFF && signature[1] == 0xD8 && signature[2] == 0xFF,
            ".png" => bytesRead == 8 && signature.AsSpan().SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            _ => false,
        };
    }

    private async Task<(string Key, string ContentType, string Path)> SaveVehicleDocumentAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var contentType = extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => throw new InvalidOperationException("Unsupported vehicle document type."),
        };
        var key = $"{Guid.NewGuid():N}{extension}";
        var directory = Path.Combine(_environment.ContentRootPath, "App_Data", "vehicle-documents");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, key);

        await using var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        await file.CopyToAsync(output);
        return (key, contentType, path);
    }

    // ── Stub pages ────────────────────────────────────────────────────────────

    public async Task<IActionResult> Fleet()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "fleet");
        ViewData["Title"] = "Fleet Routes";

        var vehicles = await _db.VehicleDetails
            .Where(v => v.VehicleOwnerId == owner.Id)
            .Include(v => v.Company)
            .Include(v => v.Routes)
            .OrderBy(v => v.RegistrationNumber)
            .ToListAsync();

        var model = new OwnerFleetRoutesViewModel
        {
            Vehicles = vehicles.Select(BuildOwnerVehicleRouteRow).ToList(),
        };

        return View(model);
    }

    private static OwnerVehicleRouteViewModel BuildOwnerVehicleRouteRow(VehicleDetail vehicle)
    {
        var route = vehicle.Routes
            .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
            .FirstOrDefault();

        return new OwnerVehicleRouteViewModel
        {
            VehicleId = vehicle.Id,
            NumberPlate = vehicle.RegistrationNumber,
            VehicleType = vehicle.VehicleType,
            CompanyName = vehicle.Company?.CompanyName ?? string.Empty,
            FromLocation = route?.FromLocation,
            ToLocation = route?.ToLocation,
            DepartureDate = route?.TripStartsAt,
            ReturnDate = route?.ReturnAvailabilityEndsAt,
            UpdatedAt = route?.UpdatedAt ?? route?.CreatedAt,
        };
    }

    public async Task<IActionResult> Earnings()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "earnings");
        ViewData["Title"] = "Earnings";
        return View("ComingSoon");
    }

    public async Task<IActionResult> Profile()
    {
        var (user, owner) = await GetContextAsync();
        SetLayoutData(user, owner, "profile");
        ViewData["Title"] = "Profile";
        return View("ComingSoon");
    }
}
