namespace OfficeSeatingPlan.Core.Entities;

public class SeatAssignment : BaseEntity
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public virtual Seat Seat { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public virtual User User { get; set; } = null!;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    //public string? AssignedById { get; set; }
    //public virtual User? AssignedBy { get; set; }

    // Navigation properties



}