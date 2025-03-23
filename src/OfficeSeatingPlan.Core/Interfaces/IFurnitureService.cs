using OfficeSeatingPlan.Core.DTOs;

namespace OfficeSeatingPlan.Core.Interfaces;

public interface IFurnitureService
{
    Task<List<FurnitureDto>> GetFurnitureByLayoutIdAsync(int layoutId);
    Task<FurnitureDto?> GetFurnitureByIdAsync(int id);
    Task<FurnitureDto> CreateFurnitureAsync(FurnitureDto furnitureDto);
    Task<FurnitureDto?> UpdateFurnitureAsync(int id, FurnitureDto furnitureDto);
    Task<bool> DeleteFurnitureAsync(int id);
}