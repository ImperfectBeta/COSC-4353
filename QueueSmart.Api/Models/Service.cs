namespace QueueSmart.Api.Models;

// PRIORITY LEVELS

public enum PriorityLevel
{
    Low,
    Medium,
    High
}

// SERVICE MODEL (we can change this later if needed)

public class Service
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public bool IsOpen { get; set; } = true;
    public int QueueLength { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
