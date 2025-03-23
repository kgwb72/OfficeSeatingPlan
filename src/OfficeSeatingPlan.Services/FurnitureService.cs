using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Data.UnitOfWork;

namespace OfficeSeatingPlan.Services;

public class FurnitureService : IFurnitureService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FurnitureService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<FurnitureDto>> GetFurnitureByLayoutIdAsync(int layoutId)
    {
        var furniture = await ((DbSet<Furniture>)_unitOfWork.FurnitureRepository.GetAllAsync().Result)
            .Where(f => f.LayoutId == layoutId)
            .ToListAsync();

        return _mapper.Map<List<FurnitureDto>>(furniture);
    }

    public async Task<FurnitureDto?> GetFurnitureByIdAsync(int id)
    {
        var furniture = await _unitOfWork.FurnitureRepository.GetByIdAsync(id);
        return furniture != null ? _mapper.Map<FurnitureDto>(furniture) : null;
    }

    public async Task<FurnitureDto> CreateFurnitureAsync(FurnitureDto furnitureDto)
    {
        var furniture = _mapper.Map<Furniture>(furnitureDto);
        var createdFurniture = await _unitOfWork.FurnitureRepository.AddAsync(furniture);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<FurnitureDto>(createdFurniture);
    }

    public async Task<FurnitureDto?> UpdateFurnitureAsync(int id, FurnitureDto furnitureDto)
    {
        var furniture = await _unitOfWork.FurnitureRepository.GetByIdAsync(id);

        if (furniture == null)
            return null;

        _mapper.Map(furnitureDto, furniture);
        furniture.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.FurnitureRepository.UpdateAsync(furniture);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FurnitureDto>(furniture);
    }

    public async Task<bool> DeleteFurnitureAsync(int id)
    {
        try
        {
            await _unitOfWork.FurnitureRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}