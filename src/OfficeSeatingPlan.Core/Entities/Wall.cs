namespace OfficeSeatingPlan.Core.Entities;

public class Wall : BaseEntity
{
    public int Id { get; set; }
    public int LayoutId { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }
    public int Thickness { get; set; } = 10;
    public string? Color { get; set; } = string.Empty;

    // Navigation properties
    public virtual Layout Layout { get; set; } = null!;
}