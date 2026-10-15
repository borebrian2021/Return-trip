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
    public string Password { get; set; } = string.Empty;

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

    [Required]
    [MaxLength(300)]
    [Display(Name = "Delivery location")]
    public string ToLocation { get; set; } = string.Empty;

    // ── Schedule ─────────────────────────────────────────────────────────────
    [Display(Name = "Required departure date")]
    public DateTime? RequiredDepartureAt { get; set; }

    [Display(Name = "Delivery deadline")]
    public DateTime? DeliveryDeadlineAt { get; set; }

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

    // ── Validation helper ─────────────────────────────────────────────────────
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

        if (RequiredDepartureAt.HasValue && DeliveryDeadlineAt.HasValue
            && DeliveryDeadlineAt < RequiredDepartureAt)
            yield return new System.ComponentModel.DataAnnotations.ValidationResult(
                "Delivery deadline must be on or after the departure date.",
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
