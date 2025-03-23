using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OfficeSeatingPlan.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<User> userManager, IMapper mapper, IConfiguration configuration)
    {
        _userManager = userManager;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = await GenerateTokenAsync(user);
        var userDto = _mapper.Map<UserDto>(user);

        // Add user roles to DTO
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();

        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            User = userDto
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            DisplayName = registerDto.DisplayName ?? $"{registerDto.FirstName} {registerDto.LastName}",
            JobTitle = registerDto.JobTitle ?? string.Empty,
            Department = registerDto.Department ?? string.Empty,
            PhoneNumber = registerDto.PhoneNumber,
            EmailConfirmed = true // For demo purposes
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errorMessage = result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}, {b}");
            throw new InvalidOperationException($"Registration failed: {errorMessage}");
        }

        // Add default role
        await _userManager.AddToRoleAsync(user, "User");

        var token = await GenerateTokenAsync(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = new List<string> { "User" };

        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            User = userDto
        };
    }

    public async Task<bool> AssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }

    private async Task<string> GenerateTokenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", user.DisplayName)
        };

        // Add roles to claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}