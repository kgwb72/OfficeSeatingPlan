using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeSeatingPlan.Core.Entities;

namespace OfficeSeatingPlan.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed roles
            await SeedRolesAsync(roleManager);

            // Seed users
            await SeedUsersAsync(userManager);

            // Seed sample data
            await SeedSampleDataAsync(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        string[] roleNames = { "Admin", "Manager", "User" };

        foreach (var roleName in roleNames)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var role = new Role
            {
                Name = roleName,
                Description = $"{roleName} role",
                NormalizedName = roleName.ToUpper()
            };

            await roleManager.CreateAsync(role);
        }
    }

    private static async Task SeedUsersAsync(UserManager<User> userManager)
    {
        // Create admin user if none exists
        if (await userManager.FindByEmailAsync("admin@example.com") == null)
        {
            var user = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                DisplayName = "Admin User",
                JobTitle = "System Administrator",
                Department = "IT",
                PhoneNumber = "555-1234"
            };

            var result = await userManager.CreateAsync(user, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        // Create manager user
        if (await userManager.FindByEmailAsync("manager@example.com") == null)
        {
            var user = new User
            {
                UserName = "manager@example.com",
                Email = "manager@example.com",
                EmailConfirmed = true,
                FirstName = "Manager",
                LastName = "User",
                DisplayName = "Manager User",
                JobTitle = "Office Manager",
                Department = "Operations",
                PhoneNumber = "555-2345"
            };

            var result = await userManager.CreateAsync(user, "Manager@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Manager");
            }
        }

        // Create regular user
        if (await userManager.FindByEmailAsync("user@example.com") == null)
        {
            var user = new User
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                EmailConfirmed = true,
                FirstName = "Regular",
                LastName = "User",
                DisplayName = "Regular User",
                JobTitle = "Software Developer",
                Department = "Engineering",
                PhoneNumber = "555-3456"
            };

            var result = await userManager.CreateAsync(user, "User@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }

    private static async Task SeedSampleDataAsync(ApplicationDbContext context)
    {
        // Seed a building if none exists
        if (!await context.Buildings.AnyAsync())
        {
            var building = new Building
            {
                Name = "Headquarters",
                Address = "123 Main Street",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Country = "USA",
                IsActive = true
            };

            context.Buildings.Add(building);
            await context.SaveChangesAsync();

            // Seed a layout
            var layout = new Layout
            {
                Name = "Main Floor",
                Description = "First floor open office layout",
                FloorNumber = 1,
                BuildingId = building.Id,
                Width = 2000,
                Height = 1500,
                IsActive = true
            };

            context.Layouts.Add(layout);
            await context.SaveChangesAsync();

            // Seed walls (basic room outline)
            var walls = new List<Wall>
            {
                new Wall { LayoutId = layout.Id, StartX = 0, StartY = 0, EndX = 2000, EndY = 0, Thickness = 10, Color = "#34495e" },
                new Wall { LayoutId = layout.Id, StartX = 2000, StartY = 0, EndX = 2000, EndY = 1500, Thickness = 10, Color = "#34495e" },
                new Wall { LayoutId = layout.Id, StartX = 2000, StartY = 1500, EndX = 0, EndY = 1500, Thickness = 10, Color = "#34495e" },
                new Wall { LayoutId = layout.Id, StartX = 0, StartY = 1500, EndX = 0, EndY = 0, Thickness = 10, Color = "#34495e" },
                // Meeting room
                new Wall { LayoutId = layout.Id, StartX = 0, StartY = 700, EndX = 500, EndY = 700, Thickness = 10, Color = "#34495e" },
                new Wall { LayoutId = layout.Id, StartX = 500, StartY = 700, EndX = 500, EndY = 0, Thickness = 10, Color = "#34495e" }
            };

            context.Walls.AddRange(walls);

            // Seed furniture (tables)
            var furniture = new List<Furniture>
            {
                // Meeting room table
                new Furniture {
                    LayoutId = layout.Id,
                    Type = "table",
                    PositionX = 250,
                    PositionY = 350,
                    Width = 300,
                    Height = 150,
                    Rotation = 0,
                    Color = "#8B4513"
                },
                // Work tables
                new Furniture {
                    LayoutId = layout.Id,
                    Type = "desk",
                    PositionX = 700,
                    PositionY = 200,
                    Width = 120,
                    Height = 60,
                    Rotation = 0,
                    Color = "#B0C4DE"
                },
                new Furniture {
                    LayoutId = layout.Id,
                    Type = "desk",
                    PositionX = 900,
                    PositionY = 200,
                    Width = 120,
                    Height = 60,
                    Rotation = 0,
                    Color = "#B0C4DE"
                },
                new Furniture {
                    LayoutId = layout.Id,
                    Type = "desk",
                    PositionX = 700,
                    PositionY = 400,
                    Width = 120,
                    Height = 60,
                    Rotation = 0,
                    Color = "#B0C4DE"
                },
                new Furniture {
                    LayoutId = layout.Id,
                    Type = "desk",
                    PositionX = 900,
                    PositionY = 400,
                    Width = 120,
                    Height = 60,
                    Rotation = 0,
                    Color = "#B0C4DE"
                }
            };

            context.Furniture.AddRange(furniture);

            // Seed seats
            var seats = new List<Seat>
            {
                new Seat { LayoutId = layout.Id, Identifier = "A101", PositionX = 700, PositionY = 170, Status = SeatStatus.Available },
                new Seat { LayoutId = layout.Id, Identifier = "A102", PositionX = 900, PositionY = 170, Status = SeatStatus.Available },
                new Seat { LayoutId = layout.Id, Identifier = "A103", PositionX = 700, PositionY = 370, Status = SeatStatus.Available },
                new Seat { LayoutId = layout.Id, Identifier = "A104", PositionX = 900, PositionY = 370, Status = SeatStatus.Available }
            };

            context.Seats.AddRange(seats);
            await context.SaveChangesAsync();
        }
    }
}