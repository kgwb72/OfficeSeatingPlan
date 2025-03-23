using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Interfaces;

namespace OfficeSeatingPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WallsController : ControllerBase
{
    private readonly IWallService _wallService;
    private readonly ILogger<WallsController> _logger;

    public WallsController(IWallService wallService, ILogger<WallsController> logger)
    {
        _wallService = wallService;
        _logger = logger;
    }

    [HttpGet("layout/{layoutId}")]
    public async Task<IActionResult> GetWallsByLayout(int layoutId)
    {
        try
        {
            var walls = await _wallService.GetWallsByLayoutIdAsync(layoutId);
            return Ok(walls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving walls for layout {LayoutId}", layoutId);
            return StatusCode(500, new { message = "An error occurred while retrieving walls" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWallById(int id)
    {
        try
        {
            var wall = await _wallService.GetWallByIdAsync(id);

            if (wall == null)
                return NotFound(new { message = "Wall not found" });

            return Ok(wall);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving wall {WallId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the wall" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateWall([FromBody] WallDto wallDto)
    {
        try
        {
            var wall = await _wallService.CreateWallAsync(wallDto);
            return CreatedAtAction(nameof(GetWallById), new { id = wall.Id }, wall);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wall");
            return StatusCode(500, new { message = "An error occurred while creating the wall" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateWall(int id, [FromBody] WallDto wallDto)
    {
        try
        {
            var wall = await _wallService.UpdateWallAsync(id, wallDto);

            if (wall == null)
                return NotFound(new { message = "Wall not found" });

            return Ok(wall);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating wall {WallId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the wall" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteWall(int id)
    {
        try
        {
            var result = await _wallService.DeleteWallAsync(id);

            if (!result)
                return NotFound(new { message = "Wall not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting wall {WallId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the wall" });
        }
    }
}