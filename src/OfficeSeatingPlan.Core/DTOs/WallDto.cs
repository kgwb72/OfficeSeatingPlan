namespace OfficeSeatingPlan.Core.DTOs;

public class WallDto
{
    public int Id { get; set; }
    public int LayoutId { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }
    public int Thickness { get; set; }
    public string? Color { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}