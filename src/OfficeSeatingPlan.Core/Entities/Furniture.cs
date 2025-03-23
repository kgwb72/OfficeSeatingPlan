using System.Text.Json;

namespace OfficeSeatingPlan.Core.Entities;

public class Furniture : BaseEntity
{
    public int Id { get; set; }
    public int LayoutId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int? Rotation { get; set; }
    public string Color { get; set; } = string.Empty;
    public string PropertiesJson { get; set; } = "{}";

    // Navigation properties
    public virtual Layout Layout { get; set; } = null!;

    // Property accessor for JSON properties
    public Dictionary<string, object> Properties
    {
        get => string.IsNullOrEmpty(PropertiesJson)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(PropertiesJson)!;
        set => PropertiesJson = JsonSerializer.Serialize(value);
    }
}