using System;
using System.Collections.Generic;
using System.Linq;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services
{
    public class QueueService : IQueueService
    {
        private static List<QueueEntry> _queue = new();
        private static int _nextId = 1;
        private readonly INotificationService _notificationService;

        private static Dictionary<int, int> _serviceDurations = new()
        {
            { 1, 10 },
            { 2, 15 },
            { 3, 5 }
        };

        public QueueService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public QueueEntryResponse JoinQueue(JoinQueueRequest request)
        {
            bool alreadyInQueue = _queue.Any(e =>
                e.UserId == request.UserId &&
                e.ServiceId == request.ServiceId &&
                e.Status == "waiting");

            if (alreadyInQueue)
                throw new InvalidOperationException("User is already in this queue.");

            var entry = new QueueEntry
            {
                Id = _nextId++,
                UserId = request.UserId,
                ServiceId = request.ServiceId,
                JoinedAt = DateTime.UtcNow,
                Priority = request.Priority,
                Status = "waiting"
            };

            _queue.Add(entry);

            // Calculate accurate wait time upon joining
            var orderedQueue = GetOrderedQueue(request.ServiceId);
            int position = orderedQueue.FindIndex(e => e.Id == entry.Id) + 1;
            int estimatedWait = CalculateWaitTime(request.ServiceId, position);

            // Trigger Notification
            _notificationService.NotifyUserJoined(request.UserId, request.ServiceId, estimatedWait);

            var response = MapToResponse(entry);
            response.Position = position;
            response.EstimatedWaitMinutes = estimatedWait;
            
            return response;
        }

        public bool LeaveQueue(int entryId, int userId)
        {
            var entry = _queue.FirstOrDefault(e =>
                e.Id == entryId &&
                e.UserId == userId &&
                e.Status == "waiting");

            if (entry == null) return false;

            entry.Status = "cancelled";
            return true;
        }

        public List<QueueEntryResponse> GetQueue(int serviceId)
        {
            var ordered = GetOrderedQueue(serviceId);
            return ordered.Select((e, index) =>
            {
                var response = MapToResponse(e);
                response.Position = index + 1;
                response.EstimatedWaitMinutes = CalculateWaitTime(serviceId, index + 1);
                return response;
            }).ToList();
        }

        public QueueEntryResponse? ServeNext(int serviceId)
        {
            var orderedQueue = GetOrderedQueue(serviceId);
            var next = orderedQueue.FirstOrDefault();
            
            if (next == null) return null;

            next.Status = "serving";

            // Check who is now at the front of the line to notify them
            var userAlmostReady = orderedQueue.Skip(1).FirstOrDefault();
            if (userAlmostReady != null)
            {
                _notificationService.NotifyUserAlmostReady(userAlmostReady.UserId, serviceId);
            }

            return MapToResponse(next);
        }

        private List<QueueEntry> GetOrderedQueue(int serviceId)
        {
            return _queue
                .Where(e => e.ServiceId == serviceId && e.Status == "waiting")
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.JoinedAt)
                .ToList();
        }

        private int CalculateWaitTime(int serviceId, int position)
        {
            int duration = _serviceDurations.GetValueOrDefault(serviceId, 10);
            return position * duration;
        }

        private QueueEntryResponse MapToResponse(QueueEntry entry)
        {
            return new QueueEntryResponse
            {
                Id = entry.Id,
                UserId = entry.UserId,
                ServiceId = entry.ServiceId,
                JoinedAt = entry.JoinedAt,
                Priority = entry.Priority,
                Status = entry.Status,
                Position = 0, 
                EstimatedWaitMinutes = 0 
            };
        }
    }
}