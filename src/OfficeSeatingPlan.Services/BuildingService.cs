using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Data.UnitOfWork;

namespace OfficeSeatingPlan.Services;

public class BuildingService : IBuildingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BuildingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<BuildingDto>> GetAllBuildingsAsync()
    {
        var buildings = await _unitOfWork.BuildingRepository.GetAllAsync();
        return _mapper.Map<List<BuildingDto>>(buildings);
    }

    public async Task<BuildingDto?> GetBuildingByIdAsync(int id)
    {
        var building = await _unitOfWork.BuildingRepository.GetByIdAsync(id);
        return building != null ? _mapper.Map<BuildingDto>(building) : null;
    }

    public async Task<BuildingDto> CreateBuildingAsync(BuildingDto buildingDto)
    {
        var building = _mapper.Map<Building>(buildingDto);
        var createdBuilding = await _unitOfWork.BuildingRepository.AddAsync(building);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BuildingDto>(createdBuilding);
    }

    public async Task<BuildingDto?> UpdateBuildingAsync(int id, BuildingDto buildingDto)
    {
        var building = await _unitOfWork.BuildingRepository.GetByIdAsync(id);

        if (building == null)
            return null;

        _mapper.Map(buildingDto, building);
        building.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.BuildingRepository.UpdateAsync(building);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BuildingDto>(building);
    }

    public async Task<bool> DeleteBuildingAsync(int id)
    {
        try
        {
            await _unitOfWork.BuildingRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}