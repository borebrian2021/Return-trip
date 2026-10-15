public class VehicleRoute
{
    public int Id { get; set; }

    public int VehicleDetailId { get; set; }
    public VehicleDetail VehicleDetail { get; set; } = null!;

    public int CompanyAgentId { get; set; }
    public CompanyAgent CompanyAgent { get; set; } = null!;

    public string FromLocation { get; set; } = string.Empty;
    public string? FromPlaceId { get; set; }
    public decimal? FromLatitude { get; set; }
    public decimal? FromLongitude { get; set; }

    public string ToLocation { get; set; } = string.Empty;
    public string? ToPlaceId { get; set; }
    public decimal? ToLatitude { get; set; }
    public decimal? ToLongitude { get; set; }

    public DateTime TripStartsAt { get; set; }
    public DateTime ExpectedArrivalAt { get; set; }
    public DateTime? ReturnAvailabilityStartsAt { get; set; }
    public DateTime? ReturnAvailabilityEndsAt { get; set; }
    public bool IsAwaitingReturn { get; set; }

    // Optional provider-neutral GeoJSON LineString used for route-corridor matching.
    public string? RoutePathJson { get; set; }
    public string? MapProvider { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<CargoBid> CargoBids { get; set; } = new List<CargoBid>();
}