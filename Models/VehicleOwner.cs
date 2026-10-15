public class VehicleOwner
{
    public int Id { get; set; }
    public int UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; } = null!;

    public string CompanyName { get; set; } = string.Empty;
    public string BusinessRegistrationNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CompanyRegistration> Companies { get; set; } = new List<CompanyRegistration>();
    public ICollection<CompanyAgent> Agents { get; set; } = new List<CompanyAgent>();
    public ICollection<VehicleDetail> Vehicles { get; set; } = new List<VehicleDetail>();
    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    public ICollection<BankDetail> BankDetails { get; set; } = new List<BankDetail>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}