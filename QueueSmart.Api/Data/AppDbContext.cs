using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserCredential> UserCredentials => Set<UserCredential>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique index on email
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
