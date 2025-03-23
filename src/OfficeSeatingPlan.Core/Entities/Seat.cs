using System.Text.Json;

namespace OfficeSeatingPlan.Core.Entities;

public class Seat : BaseEntity
{
    public int Id { get; set; }
    public int LayoutId { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int? Rotation { get; set; } = 0;
    public SeatStatus Status { get; set; } = SeatStatus.Available;
    public string? AssignedUserId { get; set; }
    public string PropertiesJson { get; set; } = "{}";

    // Navigation properties
    public virtual Layout Layout { get; set; } = null!;
    public virtual User? AssignedUser { get; set; }
    public virtual ICollection<SeatAssignment> SeatAssignments { get; set; } = new List<SeatAssignment>();
    //public virtual ICollection<SeatAssignment> Assignments { get; set; }

    //public Seat()
    //{
    //    Assignments = new HashSet<SeatAssignment>();
    //}

    // Property accessor for JSON properties
    public Dictionary<string, object> Properties
    {
        get => string.IsNullOrEmpty(PropertiesJson)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(PropertiesJson)!;
        set => PropertiesJson = JsonSerializer.Serialize(value);
    }
}

public enum SeatStatus
{
    Available,
    Occupied,
    Reserved,
    Disabled
}