using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OfficeSeatingPlan.Core.Entities;
using System.Text.Json;

namespace OfficeSeatingPlan.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Building> Buildings { get; set; }
    public DbSet<Layout> Layouts { get; set; }
    public DbSet<Furniture> Furniture { get; set; }
    public DbSet<Wall> Walls { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<SeatAssignment> SeatAssignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships and constraints
        modelBuilder.Entity<Layout>()
            .HasOne(l => l.Building)
            .WithMany(b => b.Layouts)
            .HasForeignKey(l => l.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Furniture>()
            .HasOne(f => f.Layout)
            .WithMany(l => l.Furniture)
            .HasForeignKey(f => f.LayoutId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Wall>()
            .HasOne(w => w.Layout)
            .WithMany(l => l.Walls)
            .HasForeignKey(w => w.LayoutId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Seat>()
            .HasOne(s => s.Layout)
            .WithMany(l => l.Seats)
            .HasForeignKey(s => s.LayoutId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Seat>()
            .HasOne(s => s.AssignedUser)
            .WithOne(u => u.AssignedSeat)
            .HasForeignKey<Seat>(s => s.AssignedUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SeatAssignment>()
            .HasOne(sa => sa.Seat)
            .WithMany(s => s.SeatAssignments)
            .HasForeignKey(sa => sa.SeatId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SeatAssignment>()
            .HasOne(sa => sa.User)
            .WithMany(u => u.SeatAssignments)
            .HasForeignKey(sa => sa.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        //modelBuilder.Entity<SeatAssignment>()
        //    .HasOne(sa => sa.AssignedBy)
        //    .WithMany()  // No explicit collection on the User side
        //    .HasForeignKey(sa => sa.AssignedById)
        //    .OnDelete(DeleteBehavior.SetNull)
        //    .IsRequired(false);  // Make it optional

        // Configure Properties as JSON property, not a navigation
        modelBuilder.Entity<Furniture>()
            .Property(f => f.Properties)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, object>());

        // Configure Properties as JSON property for Seat as well
        modelBuilder.Entity<Seat>()
            .Property(s => s.Properties)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, object>());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set created/updated timestamps
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }

            entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}