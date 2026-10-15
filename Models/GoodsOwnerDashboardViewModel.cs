public class GoodsOwnerDashboardViewModel
{
    // profile
    public string OwnerName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    // stat cards
    public int TotalListings { get; set; }
    public int OpenListings { get; set; }
    public int ActiveBids { get; set; }          // bids across all open cargo
    public int InTransit { get; set; }           // cargo with DepartedAt set, not yet delivered
    public int Delivered { get; set; }           // cargo with DeliveredAt set
    public int PendingPriceDecisions { get; set; } // bids awaiting goods owner price response
    public decimal TotalSpend { get; set; }      // sum of accepted bid amounts

    // recent listings for table
    public List<CargoRowViewModel> RecentListings { get; set; } = new();

    // bid activity for table
    public List<BidRowViewModel> RecentBids { get; set; } = new();

    // monthly cargo posted — for sparkline (last 6 months)
    public List<MonthlyStatViewModel> MonthlyListings { get; set; } = new();
}

public class CargoRowViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public string PackageType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int BidCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RequiredDepartureAt { get; set; }
}

public class BidRowViewModel
{
    public int BidId { get; set; }
    public int CargoId { get; set; }
    public string CargoTitle { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public string BidStatus { get; set; } = string.Empty;
    public string PriceSuggestionStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class MonthlyStatViewModel
{
    public string Month { get; set; } = string.Empty;   // "Jan", "Feb" …
    public int Count { get; set; }
}
