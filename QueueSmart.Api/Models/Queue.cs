namespace QueueSmart.Api.Models;

// queue for a service
public class Queue
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public string Status { get; set; } = "open";
    public int QueueLength { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
