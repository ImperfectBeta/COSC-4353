using System.Collections.Concurrent;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

// WHERE SERVICES ARE STORED !!!!!

public interface IServiceStore
{
    IEnumerable<Service> GetAll();
    Service? GetById(Guid id);
    Service Add(Service service);
    Service? Update(Guid id, Service service);
    bool Delete(Guid id);
}

public class ServiceStore : IServiceStore
{
    private readonly ConcurrentDictionary<Guid, Service> _services = new();

    public ServiceStore()
    {
        SeedData();
    }

    private void SeedData()
    {
        var now = DateTime.UtcNow;

        var seeds = new[]
        {
            new Service
            {
                Id = Guid.NewGuid(),
                Name = "General Consultation",
                Description = "Walk-in general medical consultation.",
                Duration = 15,
                Priority = PriorityLevel.High,
                IsOpen = true,
                QueueLength = 8,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Service
            {
                Id = Guid.NewGuid(),
                Name = "Document Processing",
                Description = "Submission and processing of official documents.",
                Duration = 10,
                Priority = PriorityLevel.Medium,
                IsOpen = true,
                QueueLength = 3,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Service
            {
                Id = Guid.NewGuid(),
                Name = "Technical Support",
                Description = "IT and device troubleshooting desk.",
                Duration = 20,
                Priority = PriorityLevel.Low,
                IsOpen = false,
                QueueLength = 0,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        foreach (var service in seeds)
        {
            _services.TryAdd(service.Id, service);
        }
    }

    public IEnumerable<Service> GetAll() => _services.Values.ToList();

    public Service? GetById(Guid id) =>
        _services.TryGetValue(id, out var service) ? service : null;

    public Service Add(Service service)
    {
        service.Id = Guid.NewGuid();
        service.CreatedAt = DateTime.UtcNow;
        service.UpdatedAt = DateTime.UtcNow;
        _services.TryAdd(service.Id, service);
        return service;
    }

    public Service? Update(Guid id, Service updated)
    {
        if (!_services.TryGetValue(id, out var existing))
            return null;

        existing.Name = updated.Name;
        existing.Description = updated.Description;
        existing.Duration = updated.Duration;
        existing.Priority = updated.Priority;
        existing.UpdatedAt = DateTime.UtcNow;

        _services[id] = existing;
        return existing;
    }

    public bool Delete(Guid id) => _services.TryRemove(id, out _);
}
