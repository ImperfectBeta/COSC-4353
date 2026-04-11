using System.ComponentModel.DataAnnotations;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.DTOs;

// SERVICE REQUESTS (validation is done here)

public class CreateServiceRequest
{
    [Required(ErrorMessage = "Service name is required.")]
    [MaxLength(100, ErrorMessage = "Service name must be 100 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(500, ErrorMessage = "Description must be 500 characters or fewer.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration is required.")]
    [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
    public int Duration { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    [EnumDataType(typeof(PriorityLevel), ErrorMessage = "Priority must be Low, Medium, or High.")]
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
}

// UPDATE SERVICE REQUEST

public class UpdateServiceRequest
{
    [Required(ErrorMessage = "Service name is required.")]
    [MaxLength(100, ErrorMessage = "Service name must be 100 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(500, ErrorMessage = "Description must be 500 characters or fewer.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration is required.")]
    [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
    public int Duration { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    [EnumDataType(typeof(PriorityLevel), ErrorMessage = "Priority must be Low, Medium, or High.")]
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
}

// SERVICE RESPONSE

public class ServiceResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Priority { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public int QueueLength { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static ServiceResponse FromService(Service service, Queue? activeQueue = null) => new()
    {
        Id = service.Id,
        Name = service.Name,
        Description = service.Description,
        Duration = service.Duration,
        Priority = service.Priority.ToString().ToLower(),
        IsOpen = activeQueue != null && activeQueue.Status == "open",
        QueueLength = activeQueue?.QueueLength ?? 0,
        CreatedAt = service.CreatedAt,
        UpdatedAt = service.UpdatedAt
    };
}
