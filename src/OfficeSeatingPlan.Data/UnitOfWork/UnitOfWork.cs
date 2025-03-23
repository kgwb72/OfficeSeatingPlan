using Microsoft.AspNetCore.Identity;
using OfficeSeatingPlan.Core.Entities;
using OfficeSeatingPlan.Data.Repositories;

namespace OfficeSeatingPlan.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private bool _disposed = false;

    // Repository instances
    private IRepository<Building>? _buildingRepository;
    private IRepository<Layout>? _layoutRepository;
    private IRepository<Furniture>? _furnitureRepository;
    private IRepository<Wall>? _wallRepository;
    private IRepository<Seat>? _seatRepository;
    private IRepository<SeatAssignment>? _seatAssignmentRepository;
    private IRepository<User>? _userRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Building> BuildingRepository =>
        _buildingRepository ??= new Repository<Building>(_context);

    public IRepository<Layout> LayoutRepository =>
        _layoutRepository ??= new Repository<Layout>(_context);

    public IRepository<Furniture> FurnitureRepository =>
        _furnitureRepository ??= new Repository<Furniture>(_context);

    public IRepository<Wall> WallRepository =>
        _wallRepository ??= new Repository<Wall>(_context);

    public IRepository<Seat> SeatRepository =>
        _seatRepository ??= new Repository<Seat>(_context);

    public IRepository<SeatAssignment> SeatAssignmentRepository =>
        _seatAssignmentRepository ??= new Repository<SeatAssignment>(_context);

    public IRepository<User> UserRepository =>
        _userRepository ??= new Repository<User>(_context);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
