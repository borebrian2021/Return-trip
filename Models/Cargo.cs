public class Cargo
{
    public int Id { get; set; }
    public int GoodsOwnerId { get; set; }
    public GoodsOwner GoodsOwner { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FromLocation { get; set; } = string.Empty;
    public string? FromPlaceId { get; set; }
    public decimal? FromLatitude { get; set; }
    public decimal? FromLongitude { get; set; }
    public string ToLocation { get; set; } = string.Empty;
    public string? ToPlaceId { get; set; }
    public decimal? ToLatitude { get; set; }
    public decimal? ToLongitude { get; set; }
    public DateTime? RequiredDepartureAt { get; set; }
    public DateTime? DeliveryDeadlineAt { get; set; }
    public DateTime? DepartedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? RetentionExpiresAt { get; set; }
    public DateTime? ReopenedAt { get; set; }
    public decimal WeightKg { get; set; }
    public string PackageType { get; set; } = string.Empty;
    public string PricingMode { get; set; } = "FixedTotal";
    public decimal? PricePerKg { get; set; }
    public decimal TransportCost { get; set; }
    public decimal CommissionPercent { get; set; }
    public int RetentionPeriodDays { get; set; }
    public string Status { get; set; } = "Open";
    public decimal AdvancePaymentPercent { get; set; } = 70;
    public bool IsBidOpen { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CargoBid> Bids { get; set; } = new List<CargoBid>();
}
