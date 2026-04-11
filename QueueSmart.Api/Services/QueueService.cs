using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services
{
    public class QueueService : IQueueService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public QueueService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<QueueEntryResponse> JoinQueue(JoinQueueRequest request)
        {
            var queue = await _context.Queues.FirstOrDefaultAsync(q => q.ServiceId == request.ServiceId);
            if (queue == null) throw new Exception("No active queue found for this service.");

            var service = await _context.Services.FindAsync(request.ServiceId);
            int duration = service?.Duration ?? 10;

            var entry = new QueueEntry
            {
                QueueId = queue.Id,
                UserId = request.UserId,
                Position = queue.QueueLength + 1,
                JoinTime = DateTime.UtcNow,
                Status = "waiting"
            };

            _context.QueueEntries.Add(entry);
            queue.QueueLength++;
            await _context.SaveChangesAsync();

            // Calculate estimated wait
            int estimatedWait = entry.Position * duration;

            // Trigger Notification
            await _notificationService.NotifyUserJoined(request.UserId, request.ServiceId, estimatedWait);

            return new QueueEntryResponse
            {
                Id = entry.QueueEntryId,
                UserId = entry.UserId,
                QueueId = entry.QueueId,
                Position = entry.Position,
                JoinTime = entry.JoinTime,
                Status = entry.Status,
                EstimatedWaitMinutes = estimatedWait
            };
        }

        public async Task<bool> LeaveQueue(int entryId, int userId)
        {
            var entry = await _context.QueueEntries.FirstOrDefaultAsync(e => e.QueueEntryId == entryId && e.UserId == userId);
            if (entry == null) return false;

            entry.Status = "cancelled";
            var queue = await _context.Queues.FindAsync(entry.QueueId);
            if (queue != null) queue.QueueLength = Math.Max(0, queue.QueueLength - 1);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<QueueEntryResponse>> GetQueue(Guid serviceId)
        {
            var queue = await _context.Queues.FirstOrDefaultAsync(q => q.ServiceId == serviceId);
            if (queue == null) return new List<QueueEntryResponse>();

            var entries = await _context.QueueEntries
                .Where(e => e.QueueId == queue.Id && e.Status == "waiting")
                .OrderBy(e => e.Position)
                .ToListAsync();

            return entries.Select(e => new QueueEntryResponse
            {
                Id = e.QueueEntryId,
                UserId = e.UserId,
                Position = e.Position,
                JoinTime = e.JoinTime,
                Status = e.Status
            }).ToList();
        }

        public async Task<QueueEntryResponse?> ServeNext(Guid serviceId)
        {
            var queue = await _context.Queues.FirstOrDefaultAsync(q => q.ServiceId == serviceId);
            if (queue == null) return null;

            var next = await _context.QueueEntries
                .Where(e => e.QueueId == queue.Id && e.Status == "waiting")
                .OrderBy(e => e.Position)
                .FirstOrDefaultAsync();

            if (next == null) return null;

            next.Status = "serving";
            
            // Notify the user who is now at position 2 that they are almost ready
            var secondInLine = await _context.QueueEntries
                .Where(e => e.QueueId == queue.Id && e.Status == "waiting" && e.Position == 2)
                .FirstOrDefaultAsync();
            
            if (secondInLine != null)
            {
                await _notificationService.NotifyUserAlmostReady(secondInLine.UserId, serviceId);
            }

            await _context.SaveChangesAsync();

            return new QueueEntryResponse { Id = next.QueueEntryId, UserId = next.UserId };
        }
    }
}