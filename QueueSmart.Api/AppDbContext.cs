using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Models;

namespace QueueSmart.Api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // enforce unique emails at the database level
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // seed data for services
        var now = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "General Consultation",
                Description = "Walk-in general medical consultation.",
                Duration = 15,
                Priority = PriorityLevel.High,
                IsOpen = true,
                QueueLength = 8,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Service
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Document Processing",
                Description = "Submission and processing of official documents.",
                Duration = 10,
                Priority = PriorityLevel.Medium,
                IsOpen = true,
                QueueLength = 3,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Service
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Technical Support",
                Description = "IT and device troubleshooting desk.",
                Duration = 20,
                Priority = PriorityLevel.Low,
                IsOpen = false,
                QueueLength = 0,
                CreatedAt = now,
                UpdatedAt = now
            }
        );
    }
}