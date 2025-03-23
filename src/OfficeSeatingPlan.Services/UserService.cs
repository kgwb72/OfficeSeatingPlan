using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Data.UnitOfWork;

namespace OfficeSeatingPlan.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .Include(u => u.AssignedSeat)
            .ThenInclude(s => s != null ? s.Layout.Building : null)
            .ToListAsync();

        var userDtos = _mapper.Map<List<UserDto>>(users);

        // Add roles to each user DTO
        foreach (var userDto in userDtos)
        {
            var user = users.First(u => u.Id == userDto.Id);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles.ToList();
        }

        return userDtos;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.Users
            .Include(u => u.AssignedSeat)
            .ThenInclude(s => s != null ? s.Layout.Building : null)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        var userDto = _mapper.Map<UserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();

        return userDto;
    }

    public async Task<UserDto?> UpdateUserAsync(string id, UserUpdateDto userDto)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return null;

        _mapper.Map(userDto, user);
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return await GetUserByIdAsync(id);
    }

    public async Task<bool> DeactivateUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ActivateUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<List<UserDto>> SearchUsersAsync(string searchTerm, string? department = null, bool? hasSeat = null)
    {
        var query = _userManager.Users
            .Include(u => u.AssignedSeat)
            .ThenInclude(s => s != null ? s.Layout.Building : null)
            .Where(u => u.DisplayName.Contains(searchTerm) ||
                        u.Email.Contains(searchTerm) ||
                        u.FirstName.Contains(searchTerm) ||
                        u.LastName.Contains(searchTerm));

        if (!string.IsNullOrEmpty(department))
        {
            query = query.Where(u => u.Department == department);
        }

        if (hasSeat.HasValue)
        {
            query = hasSeat.Value ?
                query.Where(u => u.AssignedSeat != null) :
                query.Where(u => u.AssignedSeat == null);
        }

        var users = await query.ToListAsync();
        var userDtos = _mapper.Map<List<UserDto>>(users);

        // Add roles to each user DTO
        foreach (var userDto in userDtos)
        {
            var user = users.First(u => u.Id == userDto.Id);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles.ToList();
        }

        return userDtos;
    }
}