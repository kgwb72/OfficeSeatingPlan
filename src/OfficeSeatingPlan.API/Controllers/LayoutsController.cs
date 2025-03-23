using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Services;

namespace OfficeSeatingPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LayoutsController : ControllerBase
{
    private readonly ILayoutService _layoutService;
    private readonly ILogger<LayoutsController> _logger;

    public LayoutsController(ILayoutService layoutService, ILogger<LayoutsController> logger)
    {
        _layoutService = layoutService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLayouts()
    {
        try
        {
            var layouts = await _layoutService.GetAllLayoutsAsync();
            return Ok(layouts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all layouts");
            return StatusCode(500, new { message = "An error occurred while retrieving layouts" });
        }
    }

    [HttpGet("building/{buildingId}")]
    public async Task<IActionResult> GetLayoutsByBuilding(int buildingId)
    {
        try
        {
            var layouts = await _layoutService.GetLayoutsByBuildingAsync(buildingId);
            return Ok(layouts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving layouts for building {BuildingId}", buildingId);
            return StatusCode(500, new { message = "An error occurred while retrieving layouts" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLayoutById(int id)
    {
        try
        {
            var layout = await _layoutService.GetLayoutByIdAsync(id);

            if (layout == null)
                return NotFound(new { message = "Layout not found" });

            return Ok(layout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving layout {LayoutId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the layout" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateLayout([FromBody] LayoutDto layoutDto)
    {
        try
        {
            var layout = await _layoutService.CreateLayoutAsync(layoutDto);
            return CreatedAtAction(nameof(GetLayoutById), new { id = layout.Id }, layout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating layout");
            return StatusCode(500, new { message = "An error occurred while creating the layout" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateLayout(int id, [FromBody] LayoutDto layoutDto)
    {
        try
        {
            var layout = await _layoutService.UpdateLayoutAsync(id, layoutDto);

            if (layout == null)
                return NotFound(new { message = "Layout not found" });

            return Ok(layout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating layout {LayoutId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the layout" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLayout(int id)
    {
        try
        {
            var result = await _layoutService.DeleteLayoutAsync(id);

            if (!result)
                return NotFound(new { message = "Layout not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting layout {LayoutId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the layout" });
        }
    }
}