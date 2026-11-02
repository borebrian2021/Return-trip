public class FuelAdvance
{
    public int Id { get; set; }
    public int CargoBidId { get; set; }
    public CargoBid CargoBid { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public string? EvidenceUrl { get; set; }
    public string Status { get; set; } = "PendingReconciliation";
    public int? ReconciledByCompanyAgentId { get; set; }
    public DateTime? ReconciledAt { get; set; }
}
