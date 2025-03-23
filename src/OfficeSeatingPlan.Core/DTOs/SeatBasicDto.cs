namespace OfficeSeatingPlan.Core.DTOs;

public class SeatBasicDto
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int LayoutId { get; set; }
    public string LayoutName { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public string BuildingName { get; set; } = string.Empty;
}