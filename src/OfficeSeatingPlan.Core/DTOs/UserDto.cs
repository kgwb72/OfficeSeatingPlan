namespace OfficeSeatingPlan.Core.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool HasSeatAssigned { get; set; }
    public SeatBasicDto? AssignedSeat { get; set; }
    public List<string> Roles { get; set; } = new();
}