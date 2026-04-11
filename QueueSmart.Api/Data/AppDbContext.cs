using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserCredential> UserCredentials { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Queue> Queues { get; set; } = null!;
    public DbSet<QueueEntry> QueueEntries { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Unique index on email (UserCredential)
        modelBuilder.Entity<UserCredential>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // 1-to-1: UserProfile shares PK with UserCredential
        modelBuilder.Entity<UserCredential>()
            .HasOne(c => c.Profile)
            .WithOne(p => p.Credential)
            .HasForeignKey<UserProfile>(p => p.Id);

        // seed data for services
        var now = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc);
        
        var service1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var service2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var service3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = service1Id,
                Name = "General Consultation",
                Description = "Walk-in general medical consultation.",
                Duration = 15,
                Priority = PriorityLevel.High,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Service
            {
                Id = service2Id,
                Name = "Document Processing",
                Description = "Submission and processing of official documents.",
                Duration = 10,
                Priority = PriorityLevel.Medium,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Service
            {
                Id = service3Id,
                Name = "Technical Support",
                Description = "IT and device troubleshooting desk.",
                Duration = 20,
                Priority = PriorityLevel.Low,
                CreatedAt = now,
                UpdatedAt = now
            }
        );

        // seed data for queues
        modelBuilder.Entity<Queue>().HasData(
            new Queue
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                ServiceId = service1Id,
                Status = "open",
                QueueLength = 8,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Queue
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                ServiceId = service2Id,
                Status = "open",
                QueueLength = 3,
                CreatedAt = now,
                UpdatedAt = now
            }
        );
    }
}
