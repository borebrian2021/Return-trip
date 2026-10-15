public class BankDetail
{
    public int Id { get; set; }

    public int? VehicleOwnerId { get; set; }
    public VehicleOwner? VehicleOwner { get; set; }

    public int? GoodsOwnerId { get; set; }
    public GoodsOwner? GoodsOwner { get; set; }

    public string BankName { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string SwiftCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
