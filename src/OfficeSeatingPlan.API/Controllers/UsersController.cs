using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Services;

namespace OfficeSeatingPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, IAuthService authService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return StatusCode(500, new { message = "An error occurred while retrieving users" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found" });

            // Only allow users to access their own data unless they are Admin/Manager
            if (id != User.FindFirst("sub")?.Value && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
                return Forbid();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the user" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userDto)
    {
        try
        {
            // Only allow users to update their own data unless they are Admin
            if (id != User.FindFirst("sub")?.Value && !User.IsInRole("Admin"))
                return Forbid();

            var user = await _userService.UpdateUserAsync(id, userDto);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user" });
        }
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        try
        {
            var result = await _userService.DeactivateUserAsync(id);

            if (!result)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while deactivating the user" });
        }
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        try
        {
            var result = await _userService.ActivateUserAsync(id);

            if (!result)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while activating the user" });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm, [FromQuery] string? department = null, [FromQuery] bool? hasSeat = null)
    {
        try
        {
            var users = await _userService.SearchUsersAsync(searchTerm, department, hasSeat);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            return StatusCode(500, new { message = "An error occurred while searching users" });
        }
    }

    [HttpPost("{id}/roles/{roleName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(string id, string roleName)
    {
        try
        {
            var result = await _authService.AssignRoleAsync(id, roleName);

            if (!result)
                return BadRequest(new { message = "Failed to assign role" });

            return Ok(new { message = $"Role {roleName} assigned successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", roleName, id);
            return StatusCode(500, new { message = "An error occurred while assigning the role" });
        }
    }

    [HttpDelete("{id}/roles/{roleName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveRole(string id, string roleName)
    {
        try
        {
            var result = await _authService.RemoveRoleAsync(id, roleName);

            if (!result)
                return BadRequest(new { message = "Failed to remove role" });

            return Ok(new { message = $"Role {roleName} removed successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {Role} from user {UserId}", roleName, id);
            return StatusCode(500, new { message = "An error occurred while removing the role" });
        }
    }
}