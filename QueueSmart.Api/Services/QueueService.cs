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
        private readonly IUserStore _userStore;

        public QueueService(INotificationService notificationService, IUserStore userStore)
        {
            _notificationService = notificationService;
            _userStore = userStore;
        }

        public QueueEntryResponse JoinQueue(JoinQueueRequest request)
        {
            bool alreadyInQueue = _queue.Any(e =>
                e.UserId == request.UserId &&
                e.QueueId == request.QueueId &&
                e.Status == "waiting");

            if (alreadyInQueue)
                throw new InvalidOperationException("User is already in this queue.");

            var entry = new QueueEntry
            {
                QueueEntryId = _nextId++,
                UserId = request.UserId,
                QueueId = request.QueueId,
                JoinTime = DateTime.UtcNow,
                Status = "waiting"
            };

            _queue.Add(entry);

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
            var entry = _queue.FirstOrDefault(e =>
                e.QueueEntryId == entryId &&
                e.UserId == userId &&
                e.Status == "waiting");

            if (entry == null) return false;

            entry.Status = "cancelled";
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
            var entries = _queue.Where(e => e.UserId == userId && e.Status == "waiting").ToList();
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

            // Check who is now at the front of the line to notify them
            var userAlmostReady = orderedQueue.Skip(1).FirstOrDefault();
            if (userAlmostReady != null)
            {
                _notificationService.NotifyUserAlmostReady(userAlmostReady.UserId, queueId);
            }

            return MapToResponse(next);
        }

        private List<QueueEntry> GetOrderedQueue(Guid queueId)
        {
            return _queue
                .Where(e => e.QueueId == queueId && e.Status == "waiting")
                .OrderBy(e => e.Position)
                .ThenBy(e => e.JoinTime)
                .ToList();
        }

        private int CalculateWaitTime(Guid queueId, int position)
        {
            int duration = 10; // defaults to 10 for now
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