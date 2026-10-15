public class CompanyRegistration
{
    public int Id { get; set; }

    public int VehicleOwnerId { get; set; }
    public VehicleOwner VehicleOwner { get; set; } = null!;

    public string CompanyName { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string NumberPlate { get; set; } = string.Empty;
    public string CertificateOfRegistrationUrl { get; set; } = string.Empty;
    public string CertificateOfRegistrationType { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<VehicleDetail> Vehicles { get; set; } = new List<VehicleDetail>();
    public ICollection<CompanyAgent> Agents { get; set; } = new List<CompanyAgent>();
}