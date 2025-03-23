using Microsoft.Extensions.DependencyInjection;
using OfficeSeatingPlan.Core.Interfaces;
using OfficeSeatingPlan.Data.Repositories;
using OfficeSeatingPlan.Data.UnitOfWork;

namespace OfficeSeatingPlan.Services;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register unit of work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBuildingService, BuildingService>();
        services.AddScoped<ILayoutService, LayoutService>();
        services.AddScoped<ISeatService, SeatService>();
        services.AddScoped<IFurnitureService, FurnitureService>();
        services.AddScoped<IWallService, WallService>();

        // Register AutoMapper
        services.AddAutoMapper(typeof(ServiceExtensions).Assembly);
    }
}