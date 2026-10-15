using Microsoft.AspNetCore.Identity;

public class UserAccount : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<VehicleOwner> VehicleOwners { get; set; } = new List<VehicleOwner>();
    public ICollection<CompanyAgent> CompanyAgents { get; set; } = new List<CompanyAgent>();
    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    public ICollection<GoodsOwner> GoodsOwners { get; set; } = new List<GoodsOwner>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
