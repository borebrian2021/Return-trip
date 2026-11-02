public class TripActivity
{
    public int Id { get; set; }
    public int CargoBidId { get; set; }
    public CargoBid CargoBid { get; set; } = null!;
    public int? UserAccountId { get; set; }
    public string Activity { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
