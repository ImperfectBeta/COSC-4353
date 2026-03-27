using System.Collections.Concurrent;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

// WHERE SERVICE HISTORY IS STORED

public interface IHistoryStore
{
    void Record(ServiceHistoryEntry entry);
    IEnumerable<ServiceHistoryEntry> GetAll();
    IEnumerable<ServiceHistoryEntry> GetByServiceId(Guid serviceId);
    ServiceStatisticsResponse GetStatistics(IServiceStore serviceStore);
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

    public ServiceStatisticsResponse GetStatistics(IServiceStore serviceStore)
    {
        var allServices = serviceStore.GetAll().ToList();
        return new ServiceStatisticsResponse
        {
            TotalServices = allServices.Count,
            ActiveServices = allServices.Count(s => s.IsOpen),
            TotalHistoryEntries = _entries.Count,
            RecentHistory = _entries.OrderByDescending(e => e.Timestamp).Take(20).ToList()
        };
    }
}
