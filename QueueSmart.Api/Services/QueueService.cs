using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services
{
    public class QueueService : IQueueService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IUserStore _userStore;

        public QueueService(AppDbContext context, INotificationService notificationService, IUserStore userStore)
        {
            _context = context;
            _notificationService = notificationService;
            _userStore = userStore;
        }

        public QueueEntryResponse JoinQueue(JoinQueueRequest request)
        {
            bool alreadyInQueue = _context.QueueEntries.Any(e =>
                e.UserId == request.UserId &&
                e.QueueId == request.QueueId &&
                e.Status == "waiting");

            if (alreadyInQueue)
                throw new InvalidOperationException("User is already in this queue.");

            var entry = new QueueEntry
            {
                UserId = request.UserId,
                QueueId = request.QueueId,
                JoinTime = DateTime.UtcNow,
                Status = "waiting"
            };

            _context.QueueEntries.Add(entry);
            _context.SaveChanges(); 
            
            // Calculate accurate wait time upon joining
            var orderedQueue = GetOrderedQueue(request.QueueId);
            int position = orderedQueue.FindIndex(e => e.QueueEntryId == entry.QueueEntryId) + 1;
            int estimatedWait = CalculateWaitTime(request.QueueId, position);

            // Trigger Notification
            _notificationService.NotifyUserJoined(request.UserId, request.QueueId, estimatedWait);

            var response = MapToResponse(entry);
            response.Position = position;
            response.EstimatedWaitMinutes = estimatedWait;

            return response;
        }

        public bool LeaveQueue(int entryId, int userId)
        {
            var entry = _context.QueueEntries.FirstOrDefault(e =>
                e.QueueEntryId == entryId &&
                e.UserId == userId &&
                e.Status == "waiting");

            if (entry == null) return false;

            entry.Status = "cancelled";
            _context.SaveChanges(); 
            
            return true;
        }

        public List<QueueEntryResponse> GetQueue(Guid queueId)
        {
            var ordered = GetOrderedQueue(queueId);
            return ordered.Select((e, index) =>
            {
                var response = MapToResponse(e);
                response.Position = index + 1;
                response.EstimatedWaitMinutes = CalculateWaitTime(queueId, index + 1);
                return response;
            }).ToList();
        }

        public List<QueueEntryResponse> GetUserEntries(int userId)
        {
            var entries = _context.QueueEntries
                .Where(e => e.UserId == userId && e.Status == "waiting")
                .ToList();
                
            return entries.Select(e =>
            {
                var response = MapToResponse(e);
                var orderedQueue = GetOrderedQueue(e.QueueId);
                response.Position = orderedQueue.FindIndex(x => x.QueueEntryId == e.QueueEntryId) + 1;
                response.EstimatedWaitMinutes = CalculateWaitTime(e.QueueId, response.Position);
                return response;
            }).ToList();
        }

        public QueueEntryResponse? ServeNext(Guid queueId)
        {
            var orderedQueue = GetOrderedQueue(queueId);
            var next = orderedQueue.FirstOrDefault();

            if (next == null) return null;

            next.Status = "serving";
            _context.SaveChanges();

            // Check who is now at the front of the line to notify them
            var userAlmostReady = orderedQueue.Skip(1).FirstOrDefault();
            if (userAlmostReady != null)
            {
                _notificationService.NotifyUserAlmostReady(userAlmostReady.UserId, queueId);
            }

            return MapToResponse(next);
        }

        public void ReorderQueue(Guid queueId, List<int> orderedEntryIds)
        {
            var entries = _context.QueueEntries
                .Where(e => e.QueueId == queueId && e.Status == "waiting")
                .ToList();

            var now = DateTime.UtcNow;
            
            for (int i = 0; i < orderedEntryIds.Count; i++)
            {
                var entry = entries.FirstOrDefault(e => e.QueueEntryId == orderedEntryIds[i]);
                if (entry != null)
                {
                    entry.JoinTime = now.AddSeconds(i); 
                }
            }

            _context.SaveChanges();
        }

        private List<QueueEntry> GetOrderedQueue(Guid queueId)
        {
            return _context.QueueEntries
                .Where(e => e.QueueId == queueId && e.Status == "waiting")
                .OrderBy(e => e.JoinTime) 
                .ToList();
        }

        private int CalculateWaitTime(Guid queueId, int position)
        {
            int duration = 10; 
            return position * duration;
        }

        private QueueEntryResponse MapToResponse(QueueEntry entry)
        {
            var user = _userStore.GetById(entry.UserId);
            return new QueueEntryResponse
            {
                Id = entry.QueueEntryId,
                UserId = entry.UserId,
                UserName = user?.Name ?? "Unknown User",
                QueueId = entry.QueueId,
                JoinTime = entry.JoinTime,
                Status = entry.Status,
                Position = 0,
                EstimatedWaitMinutes = 0
            };
        }
    }
}