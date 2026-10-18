public class VehicleOwnerDashboardViewModel
{
    // profile
    public string OwnerName        { get; set; } = string.Empty;
    public string CompanyName      { get; set; } = string.Empty;
    public string Location         { get; set; } = string.Empty;

    // stat cards
    public int TotalDrivers        { get; set; }
    public int VerifiedDrivers     { get; set; }
    public int TotalAgents         { get; set; }
    public int ActiveAgents        { get; set; }
    public int TotalVehicles       { get; set; }
    public int ApprovedVehicles    { get; set; }
    public int TotalCompanies      { get; set; }
    public int ActiveBids          { get; set; }   // bids placed by agents currently Pending
    public int InTransitCargo      { get; set; }   // bids Accepted with DepartedAt set
    public int DeliveredThisMonth  { get; set; }
    public decimal EarningsThisMonth { get; set; }

    // recent drivers
    public List<DriverRowViewModel>  RecentDrivers  { get; set; } = new();

    public List<Return_trip.Models.OwnerVehicleRouteViewModel> CurrentRoutes { get; set; } = new();

    // recent agents
    public List<AgentRowViewModel>   RecentAgents   { get; set; } = new();

    // recent bids placed by agents
    public List<VoBidRowViewModel>   RecentBids     { get; set; } = new();

    // monthly bids — last 6 months for chart
    public List<MonthlyStatViewModel> MonthlyBids   { get; set; } = new();
}

public class DriverRowViewModel
{
    public int      Id             { get; set; }
    public string   FullName       { get; set; } = string.Empty;
    public string   IdNumber       { get; set; } = string.Empty;
    public string   PhoneNumber    { get; set; } = string.Empty;
    public string   LicenseNumber  { get; set; } = string.Empty;
    public string?  VehicleNumberPlate { get; set; }
    public bool     IsVerified     { get; set; }
    public bool     HasAccount     { get; set; }   // UserAccountId != null
    public DateTime CreatedAt      { get; set; }
}

public class AgentRowViewModel
{
    public int      Id             { get; set; }
    public string   FullName       { get; set; } = string.Empty;
    public string   IdNumber       { get; set; } = string.Empty;
    public string   Email          { get; set; } = string.Empty;
    public string   PhoneNumber    { get; set; } = string.Empty;
    public bool     IsActive       { get; set; }
    public bool     HasAccount     { get; set; }
    public string?  CompanyName    { get; set; }
}

public class VoBidRowViewModel
{
    public int      BidId          { get; set; }
    public string   CargoTitle     { get; set; } = string.Empty;
    public string   Route          { get; set; } = string.Empty;
    public string   AgentName      { get; set; } = string.Empty;
    public decimal  BidAmount      { get; set; }
    public string   Status         { get; set; } = string.Empty;
    public DateTime CreatedAt      { get; set; }
}
