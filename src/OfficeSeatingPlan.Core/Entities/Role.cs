using Microsoft.AspNetCore.Identity;

namespace OfficeSeatingPlan.Core.Entities;

public class Role : IdentityRole
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}