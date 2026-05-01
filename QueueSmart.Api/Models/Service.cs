namespace QueueSmart.Api.Models;

// priority levels

public enum PriorityLevel
{
    Low,
    Medium,
    High
}

// service model

public class Service
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public int AdminId { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
