using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Data.UnitOfWork;

namespace OfficeSeatingPlan.Services;

public class LayoutService : ILayoutService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LayoutService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<LayoutDto>> GetAllLayoutsAsync()
    {
        var layouts = await ((DbSet<Layout>)_unitOfWork.LayoutRepository.GetAllAsync().Result)
            .Include(l => l.Building)
            .ToListAsync();

        return _mapper.Map<List<LayoutDto>>(layouts);
    }

    public async Task<List<LayoutDto>> GetLayoutsByBuildingAsync(int buildingId)
    {
        var layouts = await ((DbSet<Layout>)_unitOfWork.LayoutRepository.GetAllAsync().Result)
            .Include(l => l.Building)
            .Where(l => l.BuildingId == buildingId)
            .ToListAsync();

        return _mapper.Map<List<LayoutDto>>(layouts);
    }

    public async Task<LayoutDto?> GetLayoutByIdAsync(int id)
    {
        var layout = await ((DbSet<Layout>)_unitOfWork.LayoutRepository.GetAllAsync().Result)
            .Include(l => l.Building)
            .FirstOrDefaultAsync(l => l.Id == id);

        return layout != null ? _mapper.Map<LayoutDto>(layout) : null;
    }

    public async Task<LayoutDto> CreateLayoutAsync(LayoutDto layoutDto)
    {
        var layout = _mapper.Map<Layout>(layoutDto);
        var createdLayout = await _unitOfWork.LayoutRepository.AddAsync(layout);
        await _unitOfWork.SaveChangesAsync();

        // Fetch the complete layout with building info
        var completeLayout = await ((DbSet<Layout>)_unitOfWork.LayoutRepository.GetAllAsync().Result)
            .Include(l => l.Building)
            .FirstOrDefaultAsync(l => l.Id == createdLayout.Id);

        return _mapper.Map<LayoutDto>(completeLayout);
    }

    public async Task<LayoutDto?> UpdateLayoutAsync(int id, LayoutDto layoutDto)
    {
        var layout = await _unitOfWork.LayoutRepository.GetByIdAsync(id);

        if (layout == null)
            return null;

        _mapper.Map(layoutDto, layout);
        layout.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.LayoutRepository.UpdateAsync(layout);
        await _unitOfWork.SaveChangesAsync();

        // Fetch the complete layout with building info
        var completeLayout = await ((DbSet<Layout>)_unitOfWork.LayoutRepository.GetAllAsync().Result)
            .Include(l => l.Building)
            .FirstOrDefaultAsync(l => l.Id == id);

        return _mapper.Map<LayoutDto>(completeLayout);
    }

    public async Task<bool> DeleteLayoutAsync(int id)
    {
        try
        {
            await _unitOfWork.LayoutRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}