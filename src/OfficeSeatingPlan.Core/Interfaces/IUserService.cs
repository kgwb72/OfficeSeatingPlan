using OfficeSeatingPlan.Core.DTOs;

namespace OfficeSeatingPlan.Core.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto?> UpdateUserAsync(string id, UserUpdateDto userDto);
    Task<bool> DeactivateUserAsync(string id);
    Task<bool> ActivateUserAsync(string id);
    Task<List<UserDto>> SearchUsersAsync(string searchTerm, string? department = null, bool? hasSeat = null);
}