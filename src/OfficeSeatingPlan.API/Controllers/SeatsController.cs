using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Services;
using System.Security.Claims;

namespace OfficeSeatingPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _seatService;
    private readonly ILogger<SeatsController> _logger;

    public SeatsController(ISeatService seatService, ILogger<SeatsController> logger)
    {
        _seatService = seatService;
        _logger = logger;
    }

    [HttpGet("layout/{layoutId}")]
    public async Task<IActionResult> GetSeatsByLayout(int layoutId)
    {
        try
        {
            var seats = await _seatService.GetSeatsByLayoutIdAsync(layoutId);
            return Ok(seats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving seats for layout {LayoutId}", layoutId);
            return StatusCode(500, new { message = "An error occurred while retrieving seats" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSeatById(int id)
    {
        try
        {
            var seat = await _seatService.GetSeatByIdAsync(id);

            if (seat == null)
                return NotFound(new { message = "Seat not found" });

            return Ok(seat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving seat {SeatId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the seat" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateSeat([FromBody] SeatDto seatDto)
    {
        try
        {
            var seat = await _seatService.CreateSeatAsync(seatDto);
            return CreatedAtAction(nameof(GetSeatById), new { id = seat.Id }, seat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating seat");
            return StatusCode(500, new { message = "An error occurred while creating the seat" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateSeat(int id, [FromBody] SeatDto seatDto)
    {
        try
        {
            var seat = await _seatService.UpdateSeatAsync(id, seatDto);

            if (seat == null)
                return NotFound(new { message = "Seat not found" });

            return Ok(seat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating seat {SeatId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the seat" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteSeat(int id)
    {
        try
        {
            var result = await _seatService.DeleteSeatAsync(id);

            if (!result)
                return NotFound(new { message = "Seat not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting seat {SeatId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the seat" });
        }
    }

    [HttpPost("assign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AssignSeat([FromBody] AssignSeatDto assignDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized(new { message = "User identity not found" });

            var seat = await _seatService.AssignSeatAsync(assignDto, userId);

            if (seat == null)
                return NotFound(new { message = "Seat not found" });

            return Ok(seat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning seat {SeatId} to user {UserId}",
                assignDto.SeatId, assignDto.UserId);
            return StatusCode(500, new { message = "An error occurred while assigning the seat" });
        }
    }

    [HttpPost("{seatId}/unassign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UnassignSeat(int seatId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized(new { message = "User identity not found" });

            var seat = await _seatService.UnassignSeatAsync(seatId, userId);

            if (seat == null)
                return NotFound(new { message = "Seat not found" });

            return Ok(seat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning seat {SeatId}", seatId);
            return StatusCode(500, new { message = "An error occurred while unassigning the seat" });
        }
    }

    [HttpGet("{seatId}/history")]
    public async Task<IActionResult> GetSeatAssignmentHistory(int seatId)
    {
        try
        {
            var history = await _seatService.GetSeatAssignmentHistoryAsync(seatId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assignment history for seat {SeatId}", seatId);
            return StatusCode(500, new { message = "An error occurred while retrieving seat assignment history" });
        }
    }
}