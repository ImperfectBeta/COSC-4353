using QueueSmart.Api.Models;

namespace QueueSmart.Api.DTOs;

// SERVICE STATISTICS RESPONSE

public class ServiceStatisticsResponse
{
    public int TotalServices { get; set; }
    public int ActiveServices { get; set; }
    public int TotalHistoryEntries { get; set; }
    public List<ServiceHistoryEntry> RecentHistory { get; set; } = new();
}
