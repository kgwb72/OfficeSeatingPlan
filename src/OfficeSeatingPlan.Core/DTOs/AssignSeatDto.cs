namespace OfficeSeatingPlan.Core.DTOs;

public class AssignSeatDto
{
    public int SeatId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public class UnassignSeatDto
{
    public int SeatId { get; set; }
}