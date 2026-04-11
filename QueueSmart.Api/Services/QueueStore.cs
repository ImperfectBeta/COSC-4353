using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

public interface IQueueStore
{
    Task<IEnumerable<Queue>> GetActiveQueuesAsync();
    Task<Queue?> GetActiveQueueForServiceAsync(Guid serviceId);
    Task<Queue> OpenQueueAsync(Guid serviceId);
    Task<Queue?> CloseQueueAsync(Guid queueId);
}

public class QueueStore : IQueueStore
{
    private readonly AppDbContext _context;

    public QueueStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Queue>> GetActiveQueuesAsync()
    {
        return await _context.Queues
            .Where(q => q.Status == "open")
            .ToListAsync();
    }

    public async Task<Queue?> GetActiveQueueForServiceAsync(Guid serviceId)
    {
        return await _context.Queues
            .Where(q => q.ServiceId == serviceId && q.Status == "open")
            .FirstOrDefaultAsync();
    }

    public async Task<Queue> OpenQueueAsync(Guid serviceId)
    {
        var existing = await GetActiveQueueForServiceAsync(serviceId);
        if (existing != null)
            return existing; // already open

        var newQueue = new Queue
        {
            Id = Guid.NewGuid(),
            ServiceId = serviceId,
            Status = "open",
            QueueLength = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Queues.Add(newQueue);
        await _context.SaveChangesAsync();
        return newQueue;
    }

    public async Task<Queue?> CloseQueueAsync(Guid queueId)
    {
        var queue = await _context.Queues.FindAsync(queueId);
        if (queue == null)
            return null;

        queue.Status = "closed";
        queue.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return queue;
    }
}
