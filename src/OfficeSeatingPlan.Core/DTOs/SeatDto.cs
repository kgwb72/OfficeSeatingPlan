namespace OfficeSeatingPlan.Core.DTOs;

public class SeatDto
{
    public int Id { get; set; }
    public int LayoutId { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int? Rotation { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AssignedUserId { get; set; }
    public UserBasicDto? AssignedUser { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}