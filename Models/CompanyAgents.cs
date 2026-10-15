public class CompanyAgent
{
    public int Id { get; set; }
    public int? UserAccountId { get; set; }
    public UserAccount? UserAccount { get; set; }

    public int VehicleOwnerId { get; set; }
    public VehicleOwner VehicleOwner { get; set; } = null!;

    public int? CompanyId { get; set; }
    public CompanyRegistration? Company { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<VehicleRoute> VehicleRoutes { get; set; } = new List<VehicleRoute>();
    public ICollection<CargoBid> CargoBids { get; set; } = new List<CargoBid>();
    public ICollection<DeliveryReceipt> ConfirmedDeliveryReceipts { get; set; } = new List<DeliveryReceipt>();
}