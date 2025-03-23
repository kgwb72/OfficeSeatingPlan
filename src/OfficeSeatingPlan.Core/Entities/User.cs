using Microsoft.AspNetCore.Identity;

namespace OfficeSeatingPlan.Core.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Seat? AssignedSeat { get; set; }
    public virtual ICollection<SeatAssignment> SeatAssignments { get; set; }

    public User()
    {
        SeatAssignments = new HashSet<SeatAssignment>();
    }
    //public virtual ICollection<SeatAssignment> AssignmentsMade { get; set; }
}