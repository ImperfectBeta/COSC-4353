using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

// where service history is stored

public interface IHistoryStore
{
    Task RecordAsync(ServiceHistoryEntry entry);
    Task<IEnumerable<ServiceHistoryEntry>> GetAllAsync();
    Task<IEnumerable<ServiceHistoryEntry>> GetByServiceIdAsync(Guid serviceId);
    Task<ServiceStatisticsResponse> GetStatisticsAsync(IServiceStore serviceStore, IQueueStore queueStore, int adminId);
}

public class HistoryStore : IHistoryStore
{
    private readonly AppDbContext _context;

    public HistoryStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task RecordAsync(ServiceHistoryEntry entry)
    {
        entry.Id = Guid.NewGuid();
        entry.Timestamp = DateTime.UtcNow;
        _context.HistoryEntries.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ServiceHistoryEntry>> GetAllAsync() =>
        await _context.HistoryEntries.OrderByDescending(e => e.Timestamp).ToListAsync();

    public async Task<IEnumerable<ServiceHistoryEntry>> GetByServiceIdAsync(Guid serviceId) =>
        await _context.HistoryEntries
            .Where(e => e.ServiceId == serviceId)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();

    public async Task<ServiceStatisticsResponse> GetStatisticsAsync(IServiceStore serviceStore, IQueueStore queueStore, int adminId)
    {
        var adminServices = await serviceStore.GetByAdminIdAsync(adminId);
        var adminServiceIds = adminServices.Select(s => s.Id).ToHashSet();

        var activeQueues = await queueStore.GetActiveQueuesAsync();
        var adminActiveQueues = activeQueues.Where(q => adminServiceIds.Contains(q.ServiceId));

        var adminHistory = await _context.HistoryEntries
            .Where(e => adminServiceIds.Contains(e.ServiceId))
            .ToListAsync();

        return new ServiceStatisticsResponse
        {
            TotalServices = adminServices.Count(),
            ActiveServices = adminActiveQueues.Count(),
            TotalHistoryEntries = adminHistory.Count,
            RecentHistory = adminHistory.OrderByDescending(e => e.Timestamp).Take(20).ToList()
        };
    }
}
