using OfficeSeatingPlan.Core.DTOs;

namespace OfficeSeatingPlan.Core.Interfaces;

public interface ISeatService
{
    Task<List<SeatDto>> GetSeatsByLayoutIdAsync(int layoutId);
    Task<SeatDto?> GetSeatByIdAsync(int id);
    Task<SeatDto> CreateSeatAsync(SeatDto seatDto);
    Task<SeatDto?> UpdateSeatAsync(int id, SeatDto seatDto);
    Task<bool> DeleteSeatAsync(int id);
    Task<SeatDto?> AssignSeatAsync(AssignSeatDto assignDto, string assignedById);
    Task<SeatDto?> UnassignSeatAsync(int seatId, string assignedById);
    Task<List<SeatAssignmentDto>> GetSeatAssignmentHistoryAsync(int seatId);
}