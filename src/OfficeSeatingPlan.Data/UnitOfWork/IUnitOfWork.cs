using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Data.Repositories;

namespace OfficeSeatingPlan.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<Building> BuildingRepository { get; }
    IRepository<Layout> LayoutRepository { get; }
    IRepository<Furniture> FurnitureRepository { get; }
    IRepository<Wall> WallRepository { get; }
    IRepository<Seat> SeatRepository { get; }
    IRepository<SeatAssignment> SeatAssignmentRepository { get; }
    IRepository<User> UserRepository { get; }

    Task SaveChangesAsync();
}