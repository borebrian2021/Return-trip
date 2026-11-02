public class GoodsOwnerBidsViewModel
{
    public List<GoodsOwnerBidListItemViewModel> Bids { get; set; } = new();
}

public class GoodsOwnerBidListItemViewModel
{
    public int BidId { get; set; }
    public string CargoTitle { get; set; } = string.Empty;
    public string CargoRoute { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string VehicleRegistration { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public decimal CargoAskingPrice { get; set; }
    public decimal? SuggestedAmount { get; set; }
    public string? SuggestionComments { get; set; }
    public string PriceSuggestionStatus { get; set; } = string.Empty;
    public string BidStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal? DepositAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TripStatus { get; set; } = string.Empty;
    public decimal FinalBalance { get; set; }
    public string? FinalPaymentReference { get; set; }
    public string? FinalPaymentStatus { get; set; }
}
