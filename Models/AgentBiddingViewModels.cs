using System.ComponentModel.DataAnnotations;

public class AgentMarketplaceViewModel
{
    public List<AgentCargoOpportunityViewModel> Listings { get; set; } = new();
}

public class AgentCargoOpportunityViewModel
{
    public int CargoId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public decimal TransportCost { get; set; }
    public decimal? PricePerKg { get; set; }
    public DateTime? PickupDateTime { get; set; }
    public DateTime? PickupDeadlineAt { get; set; }
    public DateTime? DeliveryDeadlineAt { get; set; }
    public int BidCount { get; set; }
    public int? ExistingBidId { get; set; }
    public string? ExistingBidStatus { get; set; }
    public string? ExistingPriceSuggestionStatus { get; set; }
    public AgentPlaceBidViewModel BidForm { get; set; } = new();
    public List<AgentVehicleOptionViewModel> EligibleVehicles { get; set; } = new();
}

public class AgentVehicleOptionViewModel
{
    public int VehicleRouteId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public bool RequiresManualRouteCheck { get; set; }
    public DateTime DepartureAt { get; set; }
    public DateTime AvailableUntil { get; set; }
}

public class AgentPlaceBidViewModel
{
    [Range(1, int.MaxValue)]
    public int CargoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Select an eligible vehicle.")]
    [Display(Name = "Vehicle")]
    public int VehicleRouteId { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "Enter a bid greater than zero.")]
    [Display(Name = "Your bid (KSh)")]
    public decimal BidAmount { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "Enter a suggested price greater than zero.")]
    [Display(Name = "Suggested transport price (KSh)")]
    public decimal? SuggestedAmount { get; set; }

    [MaxLength(500)]
    [Display(Name = "Reason for suggested price")]
    public string? SuggestionComments { get; set; }
}

public class AgentBidsPageViewModel
{
    public List<AgentBidListItemViewModel> Bids { get; set; } = new();
}

public class AgentBidListItemViewModel
{
    public int BidId { get; set; }
    public string CargoTitle { get; set; } = string.Empty;
    public string CargoRoute { get; set; } = string.Empty;
    public string VehicleRegistration { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public decimal? SuggestedAmount { get; set; }
    public string BidStatus { get; set; } = string.Empty;
    public string PriceSuggestionStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal? DepositAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
