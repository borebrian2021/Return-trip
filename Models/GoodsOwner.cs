public class GoodsOwner
{
    public int Id { get; set; }
    public int UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Cargo> Cargoes { get; set; } = new List<Cargo>();
    public ICollection<BankDetail> BankDetails { get; set; } = new List<BankDetail>();
    public ICollection<CargoBid> PriceDecisions { get; set; } = new List<CargoBid>();
}