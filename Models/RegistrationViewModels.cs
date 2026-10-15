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
