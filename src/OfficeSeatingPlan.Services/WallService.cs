using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Data.UnitOfWork;

namespace OfficeSeatingPlan.Services;

public class WallService : IWallService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WallService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<WallDto>> GetWallsByLayoutIdAsync(int layoutId)
    {
        var walls = await ((DbSet<Wall>)_unitOfWork.WallRepository.GetAllAsync().Result)
            .Where(w => w.LayoutId == layoutId)
            .ToListAsync();

        return _mapper.Map<List<WallDto>>(walls);
    }

    public async Task<WallDto?> GetWallByIdAsync(int id)
    {
        var wall = await _unitOfWork.WallRepository.GetByIdAsync(id);
        return wall != null ? _mapper.Map<WallDto>(wall) : null;
    }

    public async Task<WallDto> CreateWallAsync(WallDto wallDto)
    {
        var wall = _mapper.Map<Wall>(wallDto);
        var createdWall = await _unitOfWork.WallRepository.AddAsync(wall);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<WallDto>(createdWall);
    }

    public async Task<WallDto?> UpdateWallAsync(int id, WallDto wallDto)
    {
        var wall = await _unitOfWork.WallRepository.GetByIdAsync(id);

        if (wall == null)
            return null;

        _mapper.Map(wallDto, wall);
        wall.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.WallRepository.UpdateAsync(wall);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<WallDto>(wall);
    }

    public async Task<bool> DeleteWallAsync(int id)
    {
        try
        {
            await _unitOfWork.WallRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}