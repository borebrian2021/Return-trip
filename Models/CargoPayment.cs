public class CargoPayment
{
    public int Id { get; set; }
    public int CargoBidId { get; set; }
    public CargoBid CargoBid { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = "PendingVerification";
    public int SubmittedByUserAccountId { get; set; }
    public int? VerifiedByCompanyAgentId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationComments { get; set; }
}
