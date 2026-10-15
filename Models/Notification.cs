public class Notification
{
    public int Id { get; set; }
    public int? UserAccountId { get; set; }
    public UserAccount? UserAccount { get; set; }
    public int? VehicleOwnerId { get; set; }
    public VehicleOwner? VehicleOwner { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
