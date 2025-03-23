using OfficeSeatingPlan.Core.DTOs;

namespace OfficeSeatingPlan.Core.Interfaces;

public interface IBuildingService
{
    Task<List<BuildingDto>> GetAllBuildingsAsync();
    Task<BuildingDto?> GetBuildingByIdAsync(int id);
    Task<BuildingDto> CreateBuildingAsync(BuildingDto buildingDto);
    Task<BuildingDto?> UpdateBuildingAsync(int id, BuildingDto buildingDto);
    Task<bool> DeleteBuildingAsync(int id);
}