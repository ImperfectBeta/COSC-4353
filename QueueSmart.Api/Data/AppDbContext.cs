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
    public DbSet<ServiceHistoryEntry> HistoryEntries { get; set; } = null!;

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
    }
}