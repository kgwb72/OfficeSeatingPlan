using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Data.UnitOfWork;

namespace OfficeSeatingPlan.Services;

public class SeatService : ISeatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SeatService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<SeatDto>> GetSeatsByLayoutIdAsync(int layoutId)
    {
        var seats = await ((DbSet<Seat>)_unitOfWork.SeatRepository.GetAllAsync().Result)
            .Include(s => s.AssignedUser)
            .Where(s => s.LayoutId == layoutId)
            .ToListAsync();

        return _mapper.Map<List<SeatDto>>(seats);
    }

    public async Task<SeatDto?> GetSeatByIdAsync(int id)
    {
        var seat = await ((DbSet<Seat>)_unitOfWork.SeatRepository.GetAllAsync().Result)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == id);

        return seat != null ? _mapper.Map<SeatDto>(seat) : null;
    }

    public async Task<SeatDto> CreateSeatAsync(SeatDto seatDto)
    {
        var seat = _mapper.Map<Seat>(seatDto);
        var createdSeat = await _unitOfWork.SeatRepository.AddAsync(seat);
        await _unitOfWork.SaveChangesAsync();

        var completeSeat = await ((DbSet<Seat>)_unitOfWork.SeatRepository.GetAllAsync().Result)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == createdSeat.Id);

        return _mapper.Map<SeatDto>(completeSeat);
    }

    public async Task<SeatDto?> UpdateSeatAsync(int id, SeatDto seatDto)
    {
        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(id);

        if (seat == null)
            return null;

        // We don't want to change the assigned user this way
        var currentAssignedUserId = seat.AssignedUserId;

        _mapper.Map(seatDto, seat);
        seat.UpdatedAt = DateTime.UtcNow;
        seat.AssignedUserId = currentAssignedUserId; // Preserve the assigned user

        await _unitOfWork.SeatRepository.UpdateAsync(seat);
        await _unitOfWork.SaveChangesAsync();

        var completeSeat = await ((DbSet<Seat>)_unitOfWork.SeatRepository.GetAllAsync().Result)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == id);

        return _mapper.Map<SeatDto>(completeSeat);
    }

    public async Task<bool> DeleteSeatAsync(int id)
    {
        try
        {
            await _unitOfWork.SeatRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<SeatDto?> AssignSeatAsync(AssignSeatDto assignDto, string assignedById)
    {
        var seat = await ((DbSet<Seat>)_unitOfWork.SeatRepository.GetAllAsync().Result)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == assignDto.SeatId);

        if (seat == null)
            return null;

        // If seat is already assigned to this user, do nothing
        if (seat.AssignedUserId == assignDto.UserId)
            return _mapper.Map<SeatDto>(seat);

        // End current assignment if any
        if (!string.IsNullOrEmpty(seat.AssignedUserId))
        {
            var currentAssignment = await ((DbSet<SeatAssignment>)_unitOfWork.SeatAssignmentRepository.GetAllAsync().Result)
                .Where(sa => sa.SeatId == seat.Id && sa.EndDate == null)
                .FirstOrDefaultAsync();

            if (currentAssignment != null)
            {
                currentAssignment.EndDate = DateTime.UtcNow;
                await _unitOfWork.SeatAssignmentRepository.UpdateAsync(currentAssignment);
            }
        }

        // Create new seat assignment history record
        var seatAssignment = new SeatAssignment
        {
            SeatId = seat.Id,
            UserId = assignDto.UserId,
            StartDate = DateTime.UtcNow,
            //AssignedById = assignedById
        };

        await _unitOfWork.SeatAssignmentRepository.AddAsync(seatAssignment);

        // Update seat status
        seat.AssignedUserId = assignDto.UserId;
        seat.Status = SeatStatus.Occupied;
        seat.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SeatRepository.UpdateAsync(seat);
        await _unitOfWork.SaveChangesAsync();

        var updatedSeat = await ((DbSet<Seat>)_unitOfWork.SeatRepository.GetAllAsync().Result)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == assignDto.SeatId);

        return _mapper.Map<SeatDto>(updatedSeat);
    }

    public async Task<SeatDto?> UnassignSeatAsync(int seatId, string assignedById)
    {
        var seat = await ((DbSet<Seat>)_unitOfWork.SeatRepository.GetAllAsync().Result)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == seatId);

        if (seat == null || string.IsNullOrEmpty(seat.AssignedUserId))
            return seat != null ? _mapper.Map<SeatDto>(seat) : null;

        // End current assignment
        var currentAssignment = await ((DbSet<SeatAssignment>)_unitOfWork.SeatAssignmentRepository.GetAllAsync().Result)
            .Where(sa => sa.SeatId == seat.Id && sa.EndDate == null)
            .FirstOrDefaultAsync();

        if (currentAssignment != null)
        {
            currentAssignment.EndDate = DateTime.UtcNow;
            await _unitOfWork.SeatAssignmentRepository.UpdateAsync(currentAssignment);
        }

        // Update seat status
        seat.AssignedUserId = null;
        seat.Status = SeatStatus.Available;
        seat.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SeatRepository.UpdateAsync(seat);
        await _unitOfWork.SaveChangesAsync();

        var updatedSeat = await ((DbSet<Seat>)_unitOfWork.SeatRepository.GetAllAsync().Result)
            .FirstOrDefaultAsync(s => s.Id == seatId);

        return _mapper.Map<SeatDto>(updatedSeat);
    }

    public async Task<List<SeatAssignmentDto>> GetSeatAssignmentHistoryAsync(int seatId)
    {
        var assignments = await ((DbSet<SeatAssignment>)_unitOfWork.SeatAssignmentRepository.GetAllAsync().Result)
            .Include(sa => sa.User)
            .Include(sa => sa.Seat)
            //.Include(sa => sa.AssignedBy)
            .Where(sa => sa.SeatId == seatId)
            .OrderByDescending(sa => sa.StartDate)
            .ToListAsync();

        return _mapper.Map<List<SeatAssignmentDto>>(assignments);
    }
}