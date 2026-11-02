public class CargoBid
{
    public int Id { get; set; }
    public int CargoId { get; set; }
    public Cargo Cargo { get; set; } = null!;

    public int CompanyAgentId { get; set; }
    public CompanyAgent CompanyAgent { get; set; } = null!;

    public int VehicleRouteId { get; set; }
    public VehicleRoute VehicleRoute { get; set; } = null!;

    public int? DriverId { get; set; }
    public Driver? Driver { get; set; }

    public decimal BidAmount { get; set; }
    public decimal? SuggestedAmount { get; set; }
    public string? SuggestionComments { get; set; }
    public string PriceSuggestionStatus { get; set; } = "NotRequired";
    public DateTime? PriceDecisionAt { get; set; }
    public int? PriceDecidedByGoodsOwnerId { get; set; }
    public GoodsOwner? PriceDecidedByGoodsOwner { get; set; }
    public string PaymentStatus { get; set; } = "NotStarted";
    public decimal? DepositAmount { get; set; }
    public DateTime? DepositPaidAt { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime? EstimatedVehicleArrivalAt { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string TripStatus { get; set; } = "AwaitingDeposit";
    public DateTime? TripAssignedAt { get; set; }

    public DeliveryReceipt? DeliveryReceipt { get; set; }
    public ICollection<CargoPayment> Payments { get; set; } = new List<CargoPayment>();
    public ICollection<FuelAdvance> FuelAdvances { get; set; } = new List<FuelAdvance>();
    public ICollection<TripActivity> Activities { get; set; } = new List<TripActivity>();
}
