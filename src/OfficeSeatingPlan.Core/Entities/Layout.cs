namespace OfficeSeatingPlan.Core.Entities;

public class Layout : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public int BuildingId { get; set; }
    public int Width { get; set; } = 2000;
    public int Height { get; set; } = 1500;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Building Building { get; set; } = null!;
    public virtual ICollection<Furniture> Furniture { get; set; } = new List<Furniture>();
    public virtual ICollection<Wall> Walls { get; set; } = new List<Wall>();
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}