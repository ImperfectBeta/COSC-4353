namespace QueueSmart.Api.Models;

// SERVICE HISTORY MODEL

public class ServiceHistoryEntry
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}
