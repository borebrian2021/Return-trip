public class AdminCommissionSetting
{
    public int Id { get; set; }
    public decimal Percentage { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
