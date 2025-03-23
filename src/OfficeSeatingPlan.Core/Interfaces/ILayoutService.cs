using OfficeSeatingPlan.Core.DTOs;

namespace OfficeSeatingPlan.Core.Interfaces;

public interface ILayoutService
{
    Task<List<LayoutDto>> GetAllLayoutsAsync();
    Task<List<LayoutDto>> GetLayoutsByBuildingAsync(int buildingId);
    Task<LayoutDto?> GetLayoutByIdAsync(int id);
    Task<LayoutDto> CreateLayoutAsync(LayoutDto layoutDto);
    Task<LayoutDto?> UpdateLayoutAsync(int id, LayoutDto layoutDto);
    Task<bool> DeleteLayoutAsync(int id);
}