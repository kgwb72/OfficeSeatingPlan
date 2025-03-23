namespace OfficeSeatingPlan.Core.DTOs;

public class LayoutDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public int BuildingId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}