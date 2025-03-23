using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Services;

namespace OfficeSeatingPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FurnitureController : ControllerBase
{
    private readonly IFurnitureService _furnitureService;
    private readonly ILogger<FurnitureController> _logger;

    public FurnitureController(IFurnitureService furnitureService, ILogger<FurnitureController> logger)
    {
        _furnitureService = furnitureService;
        _logger = logger;
    }

    [HttpGet("layout/{layoutId}")]
    public async Task<IActionResult> GetFurnitureByLayout(int layoutId)
    {
        try
        {
            var furniture = await _furnitureService.GetFurnitureByLayoutIdAsync(layoutId);
            return Ok(furniture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving furniture for layout {LayoutId}", layoutId);
            return StatusCode(500, new { message = "An error occurred while retrieving furniture" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFurnitureById(int id)
    {
        try
        {
            var furniture = await _furnitureService.GetFurnitureByIdAsync(id);

            if (furniture == null)
                return NotFound(new { message = "Furniture not found" });

            return Ok(furniture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving furniture {FurnitureId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the furniture" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateFurniture([FromBody] FurnitureDto furnitureDto)
    {
        try
        {
            var furniture = await _furnitureService.CreateFurnitureAsync(furnitureDto);
            return CreatedAtAction(nameof(GetFurnitureById), new { id = furniture.Id }, furniture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating furniture");
            return StatusCode(500, new { message = "An error occurred while creating furniture" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateFurniture(int id, [FromBody] FurnitureDto furnitureDto)
    {
        try
        {
            var furniture = await _furnitureService.UpdateFurnitureAsync(id, furnitureDto);

            if (furniture == null)
                return NotFound(new { message = "Furniture not found" });

            return Ok(furniture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating furniture {FurnitureId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the furniture" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteFurniture(int id)
    {
        try
        {
            var result = await _furnitureService.DeleteFurnitureAsync(id);

            if (!result)
                return NotFound(new { message = "Furniture not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting furniture {FurnitureId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the furniture" });
        }
    }
}