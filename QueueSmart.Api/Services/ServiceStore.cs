using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

// where services are stored

public interface IServiceStore
{
    Task<IEnumerable<Service>> GetAllAsync();
    Task<Service?> GetByIdAsync(Guid id);
    Task<Service> AddAsync(Service service);
    Task<Service?> UpdateAsync(Guid id, Service service);
    Task<bool> DeleteAsync(Guid id);
}

public class ServiceStore : IServiceStore
{
    private readonly AppDbContext _context;

    public ServiceStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllAsync()
    {
        return await _context.Services.ToListAsync();
    }

    public async Task<Service?> GetByIdAsync(Guid id)
    {
        return await _context.Services.FindAsync(id);
    }

    public async Task<Service> AddAsync(Service service)
    {
        service.Id = Guid.NewGuid();
        service.CreatedAt = DateTime.UtcNow;
        service.UpdatedAt = DateTime.UtcNow;

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return service;
    }

    public async Task<Service?> UpdateAsync(Guid id, Service updated)
    {
        var existing = await _context.Services.FindAsync(id);
        if (existing == null)
            return null;

        existing.Name = updated.Name;
        existing.Description = updated.Description;
        existing.Duration = updated.Duration;
        existing.Priority = updated.Priority;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return false;

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }
}
