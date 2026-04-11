using System.Collections.Concurrent;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

// where service history is stored

public interface IHistoryStore
{
    void Record(ServiceHistoryEntry entry);
    IEnumerable<ServiceHistoryEntry> GetAll();
    IEnumerable<ServiceHistoryEntry> GetByServiceId(Guid serviceId);
    Task<ServiceStatisticsResponse> GetStatisticsAsync(IServiceStore serviceStore, IQueueStore queueStore);
}

public class HistoryStore : IHistoryStore
{
    private readonly ConcurrentBag<ServiceHistoryEntry> _entries = new();

    public void Record(ServiceHistoryEntry entry)
    {
        entry.Id = Guid.NewGuid();
        entry.Timestamp = DateTime.UtcNow;
        _entries.Add(entry);
    }

    public IEnumerable<ServiceHistoryEntry> GetAll() =>
        _entries.OrderByDescending(e => e.Timestamp).ToList();

    public IEnumerable<ServiceHistoryEntry> GetByServiceId(Guid serviceId) =>
        _entries.Where(e => e.ServiceId == serviceId)
                .OrderByDescending(e => e.Timestamp)
                .ToList();

    public async Task<ServiceStatisticsResponse> GetStatisticsAsync(IServiceStore serviceStore, IQueueStore queueStore)
    {
        var allServices = await serviceStore.GetAllAsync();
        var activeQueues = await queueStore.GetActiveQueuesAsync();

        return new ServiceStatisticsResponse
        {
            TotalServices = allServices.Count(),
            ActiveServices = activeQueues.Count(),
            TotalHistoryEntries = _entries.Count,
            RecentHistory = _entries.OrderByDescending(e => e.Timestamp).Take(20).ToList()
        };
    }
}
