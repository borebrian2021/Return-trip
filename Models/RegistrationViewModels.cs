using System.ComponentModel.DataAnnotations;

namespace Return_trip.Models;

public class GoodsOwnerRegistrationViewModel
{
    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Use at least 8 characters, including an uppercase letter, a lowercase letter, and a number.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Business / trading name")]
    public string BusinessName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;
}

public class VehicleOwnerRegistrationViewModel
{
    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Use at least 8 characters, including an uppercase letter, a lowercase letter, and a number.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Company name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Business registration number")]
    public string BusinessRegistrationNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Operations location")]
    public string Location { get; set; } = string.Empty;
}

public class DriverIdLookupViewModel
{
    [Required]
    [Display(Name = "National ID number")]
    public string IdNumber { get; set; } = string.Empty;
}

public class DriverClaimViewModel
{
    [Required]
    [Display(Name = "National ID number")]
    public string IdNumber { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Email address registered by your vehicle owner")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(8)]
    [Display(Name = "Create password")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string ProfileName { get; set; } = string.Empty;
}

public class DriverProfileViewModel
{
    [Required]
    [Display(Name = "Driver full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "National ID number")]
    public string IdNumber { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Driver email")]
    public string Email { get; set; } = string.Empty;

    [Required, Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Driver licence number")]
    public string LicenseNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "Assigned vehicle")]
    public int VehicleDetailId { get; set; }
}

public class CompanyRegistrationViewModel
{
    [Required]
    [Display(Name = "Company name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Company registration number")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Business sector")]
    public string BusinessSector { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Company email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Company phone")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Operating location")]
    public string Location { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Certificate of registration")]
    public IFormFile? CertificateOfRegistration { get; set; }
}

public class VehicleDetailViewModel
{
    [Required]
    [Display(Name = "Vehicle type")]
    public string VehicleType { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Vehicle number plate")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Company this vehicle belongs to")]
    public int CompanyId { get; set; }

    [Required]
    [Display(Name = "Insurance scanned document")]
    public IFormFile? InsuranceDocument { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Insurance expiry date")]
    public DateTime? InsuranceExpiryDate { get; set; }

    [Required]
    [Display(Name = "Inspection report scanned document")]
    public IFormFile? InspectionReport { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Inspection expiry date")]
    public DateTime? InspectionExpiryDate { get; set; }

    [Required]
    [Range(0.01, 1000000)]
    [Display(Name = "Vehicle capacity (tonnes)")]
    public decimal? Capacity { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "Last cargo transported")]
    public string LastCargoTransported { get; set; } = string.Empty;
}

public class AgentRoutesPageViewModel
{
    public List<AgentVehicleRouteViewModel> Vehicles { get; set; } = new();
}

public class AgentVehicleRouteViewModel
{
    public int VehicleId { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public string NumberPlate { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public DateTime? DepartureDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}

public class AgentRouteUpdateViewModel
{
    [Range(1, int.MaxValue)]
    public int VehicleId { get; set; }

    [Required, MaxLength(300)]
    [Display(Name = "From location")]
    public string FromLocation { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    [Display(Name = "To location")]
    public string ToLocation { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Departure date")]
    public DateTime? DepartureDate { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Return date")]
    public DateTime? ReturnDate { get; set; }
}

public class LoginViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Keep me signed in")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class AgentProfileViewModel
{
    [Required]
    [Display(Name = "Agent full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Agent email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Agent phone")]
    public string PhoneNumber { get; set; } = string.Empty;

}

public class AgentEmailLookupViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email address registered by your vehicle owner")]
    public string Email { get; set; } = string.Empty;
}

public class AgentClaimViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Registered email address")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(8)]
    [Display(Name = "Create password")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string ProfileName { get; set; } = string.Empty;
}

public class RegistrationSuccessViewModel
{
    public string RoleLabel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class PostCargoViewModel
{
    // ── What are you shipping? ──────────────────────────────────────────────
    [Required]
    [Display(Name = "Cargo title")]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Description")]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Package / cargo type")]
    public string PackageType { get; set; } = string.Empty;

    [Required]
    [Range(0.001, 100000, ErrorMessage = "Weight must be greater than 0.")]
    [Display(Name = "Weight (kg)")]
    public decimal WeightKg { get; set; }

    // ── Pricing ─────────────────────────────────────────────────────────────
    [Required]
    [Display(Name = "Pricing mode")]
    public string PricingMode { get; set; } = "FixedTotal";

    [Range(0.01, double.MaxValue, ErrorMessage = "Transport cost must be greater than 0.")]
    [Display(Name = "Total transport cost (R)")]
    public decimal? TransportCost { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price per kg must be greater than 0.")]
    [Display(Name = "Price per kg (R)")]
    public decimal? PricePerKg { get; set; }

    // ── Route ────────────────────────────────────────────────────────────────
    [Required]
    [MaxLength(300)]
    [Display(Name = "Pickup location")]
    public string FromLocation { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FromPlaceId { get; set; }
    public decimal? FromLatitude { get; set; }
    public decimal? FromLongitude { get; set; }

    [Required]
    [MaxLength(300)]
    [Display(Name = "Delivery location")]
    public string ToLocation { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ToPlaceId { get; set; }
    public decimal? ToLatitude { get; set; }
    public decimal? ToLongitude { get; set; }

    [Range(0.1, 100000, ErrorMessage = "Enter a valid distance in kilometres.")]
    [Display(Name = "Distance (km)")]
    public decimal? DistanceKm { get; set; }

    public bool DistanceAutoCalculated { get; set; } = false;

    // ── Schedule ─────────────────────────────────────────────────────────────
    [Required(ErrorMessage = "Pickup date and time is required.")]
    [Display(Name = "Pickup date & time")]
    public DateTime? PickupDateTime { get; set; }

    [Required(ErrorMessage = "Pickup deadline (latest acceptable pickup time) is required.")]
    [Display(Name = "Latest pickup by (deadline)")]
    public DateTime? PickupDeadlineAt { get; set; }

    [Display(Name = "Delivery deadline")]
    public DateTime? DeliveryDeadlineAt { get; set; }

    // ── Goods owner contact ───────────────────────────────────────────────────
    [Required(ErrorMessage = "Your contact name is required.")]
    [Display(Name = "Your contact name")]
    [MaxLength(150)]
    public string SenderContactName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Your phone number is required.")]
    [Phone]
    [Display(Name = "Your phone number")]
    [MaxLength(20)]
    public string SenderPhone { get; set; } = string.Empty;

    // ── Receiver contact (revealed to driver after advance payment) ───────────
    [Required(ErrorMessage = "Receiver name is required.")]
    [Display(Name = "Receiver name")]
    [MaxLength(150)]
    public string ReceiverName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Receiver phone number is required.")]
    [Phone]
    [Display(Name = "Receiver phone")]
    [MaxLength(20)]
    public string ReceiverPhone { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Receiver email (optional)")]
    [MaxLength(200)]
    public string? ReceiverEmail { get; set; }

    // ── Payment terms ─────────────────────────────────────────────────────────
    [Required]
    [Range(1, 90, ErrorMessage = "Retention period must be between 1 and 90 days.")]
    [Display(Name = "Payment retention period (days)")]
    public int RetentionPeriodDays { get; set; } = 7;

    [Required]
    [Range(1, 100, ErrorMessage = "Advance payment must be between 1% and 100%.")]
    [Display(Name = "Advance payment to driver (%)")]
    public decimal AdvancePaymentPercent { get; set; } = 70;

    // read-only — populated from AdminCommissionSettings
    public decimal CommissionPercent { get; set; }

    public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(
        System.ComponentModel.DataAnnotations.ValidationContext context)
    {
        if (PricingMode == "FixedTotal" && (TransportCost == null || TransportCost <= 0))
            yield return new System.ComponentModel.DataAnnotations.ValidationResult(
                "Enter a total transport cost for Fixed Total pricing.",
                new[] { nameof(TransportCost) });

        if (PricingMode == "PerKg" && (PricePerKg == null || PricePerKg <= 0))
            yield return new System.ComponentModel.DataAnnotations.ValidationResult(
                "Enter a price per kg for Per-Kg pricing.",
                new[] { nameof(PricePerKg) });

        if (PickupDateTime.HasValue && PickupDeadlineAt.HasValue && PickupDeadlineAt < PickupDateTime)
            yield return new System.ComponentModel.DataAnnotations.ValidationResult(
                "Latest pickup deadline must be after the scheduled pickup time.",
                new[] { nameof(PickupDeadlineAt) });

        if (PickupDateTime.HasValue && DeliveryDeadlineAt.HasValue && DeliveryDeadlineAt < PickupDateTime)
            yield return new System.ComponentModel.DataAnnotations.ValidationResult(
                "Delivery deadline must be on or after the pickup date.",
                new[] { nameof(DeliveryDeadlineAt) });
    }
}

public class MyListingsViewModel
{
    public List<CargoListingRow> Listings { get; set; } = new();
    public int TotalCount { get; set; }
    public int OpenCount { get; set; }
    public int InTransitCount { get; set; }
    public int DeliveredCount { get; set; }
    public string? FilterStatus { get; set; }
}

public class CargoListingRow
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public string PricingMode { get; set; } = string.Empty;
    public decimal TransportCost { get; set; }
    public decimal? PricePerKg { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsBidOpen { get; set; }
    public int BidCount { get; set; }
    public int AcceptedBidCount { get; set; }
    public DateTime? RequiredDepartureAt { get; set; }
    public DateTime? DeliveryDeadlineAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
