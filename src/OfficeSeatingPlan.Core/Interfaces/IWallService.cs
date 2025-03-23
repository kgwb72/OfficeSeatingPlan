using OfficeSeatingPlan.Core.DTOs;

namespace OfficeSeatingPlan.Core.Interfaces;

public interface IWallService
{
    Task<List<WallDto>> GetWallsByLayoutIdAsync(int layoutId);
    Task<WallDto?> GetWallByIdAsync(int id);
    Task<WallDto> CreateWallAsync(WallDto wallDto);
    Task<WallDto?> UpdateWallAsync(int id, WallDto wallDto);
    Task<bool> DeleteWallAsync(int id);
}