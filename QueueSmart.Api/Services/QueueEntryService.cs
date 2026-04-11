using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

public class QueueEntryService : IQueueEntryService
{
    private readonly ILogger<QueueEntryService> _logger;
    private readonly AppDbContext _context;

    public QueueEntryService(ILogger<QueueEntryService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public QueueEntry JoinQueue(Guid queueId, int userId)
    {
        int nextPosition = _context.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == "waiting")
            .Count() + 1;

        var entry = new QueueEntry
        {
            QueueId = queueId,
            UserId = userId,
            Position = nextPosition,
            JoinTime = DateTime.UtcNow,
            Status = "waiting"
        };

        _context.QueueEntries.Add(entry);
        _context.SaveChanges();

        _logger.LogInformation("User {UserId} joined queue {QueueId} at position  {position}",
            userId, queueId, nextPosition);

        return entry;
    }

    public bool CancelEntry(int queueEntryId, int userId)
    {
        var entry = _context.QueueEntries
            .FirstOrDefault(e => e.QueueEntryId == queueEntryId && e.UserId == userId);
        
        if (entry == null) return false;

        entry.Status = "cancelled";
        _context.SaveChanges();

        var behind = _context.QueueEntries
            .Where(e => e.QueueId == entry.QueueId
                && e.Status == "waiting"
                && e.Position > entry.Position)
            .ToList();
        
        foreach (var e in behind) e.Position--;
        _context.SaveChanges();

        return true;
    }

    public List<QueueEntry> GetQueueEntries(Guid queueId)
    {
        return _context.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == "waiting")
            .OrderBy(e => e.Position)
            .ToList();
    }

    public List<QueueEntry> GetHistoryForUser(int userId)
    {
        return _context.QueueEntries
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.JoinTime)
            .ToList();
    }
}
