using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Services;

namespace OfficeSeatingPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BuildingsController : ControllerBase
{
    private readonly IBuildingService _buildingService;
    private readonly ILogger<BuildingsController> _logger;

    public BuildingsController(IBuildingService buildingService, ILogger<BuildingsController> logger)
    {
        _buildingService = buildingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBuildings()
    {
        try
        {
            var buildings = await _buildingService.GetAllBuildingsAsync();
            return Ok(buildings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all buildings");
            return StatusCode(500, new { message = "An error occurred while retrieving buildings" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBuildingById(int id)
    {
        try
        {
            var building = await _buildingService.GetBuildingByIdAsync(id);

            if (building == null)
                return NotFound(new { message = "Building not found" });

            return Ok(building);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving building {BuildingId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the building" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateBuilding([FromBody] BuildingDto buildingDto)
    {
        try
        {
            var building = await _buildingService.CreateBuildingAsync(buildingDto);
            return CreatedAtAction(nameof(GetBuildingById), new { id = building.Id }, building);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating building");
            return StatusCode(500, new { message = "An error occurred while creating the building" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateBuilding(int id, [FromBody] BuildingDto buildingDto)
    {
        try
        {
            var building = await _buildingService.UpdateBuildingAsync(id, buildingDto);

            if (building == null)
                return NotFound(new { message = "Building not found" });

            return Ok(building);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating building {BuildingId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the building" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBuilding(int id)
    {
        try
        {
            var result = await _buildingService.DeleteBuildingAsync(id);

            if (!result)
                return NotFound(new { message = "Building not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting building {BuildingId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the building" });
        }
    }
}