public class VehicleDetail
{
    public int Id { get; set; }

    public int VehicleOwnerId { get; set; }
    public VehicleOwner VehicleOwner { get; set; } = null!;

    public int CompanyId { get; set; }
    public CompanyRegistration Company { get; set; } = null!;

    public string VehicleType { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseDocumentUrl { get; set; } = string.Empty;
    public string LicenseDocumentType { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<VehicleRoute> Routes { get; set; } = new List<VehicleRoute>();
}