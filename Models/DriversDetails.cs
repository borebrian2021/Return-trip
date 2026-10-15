public class Driver
{
    public int Id { get; set; }
    public int? UserAccountId { get; set; }
    public UserAccount? UserAccount { get; set; }

    public int VehicleOwnerId { get; set; }
    public VehicleOwner VehicleOwner { get; set; } = null!;

    public string FullName { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseImageUrl { get; set; } = string.Empty;
    public string LicenseImageType { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CargoBid> Bids { get; set; } = new List<CargoBid>();
    public ICollection<DeliveryReceipt> UploadedDeliveryReceipts { get; set; } = new List<DeliveryReceipt>();
}