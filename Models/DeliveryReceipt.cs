public class DeliveryReceipt
{
    public int Id { get; set; }

    public int CargoBidId { get; set; }
    public CargoBid CargoBid { get; set; } = null!;

    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    public string ReceiverName { get; set; } = string.Empty;
    public DateTime SignedAt { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageContentType { get; set; } = string.Empty;
    public string? ImageFileName { get; set; }

    public string ConfirmationStatus { get; set; } = "Pending";
    public int? ConfirmedByCompanyAgentId { get; set; }
    public CompanyAgent? ConfirmedByCompanyAgent { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string? AgentComments { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}