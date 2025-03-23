namespace OfficeSeatingPlan.Core.DTOs;

public class FurnitureDto
{
    public int Id { get; set; }
    public int LayoutId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Rotation { get; set; }
    public string Color { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}