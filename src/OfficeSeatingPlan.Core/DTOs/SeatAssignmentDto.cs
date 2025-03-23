namespace OfficeSeatingPlan.Core.DTOs;

public class SeatAssignmentDto
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public string SeatIdentifier { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    //public string? AssignedById { get; set; }
    //public string? AssignedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}