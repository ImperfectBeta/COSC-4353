using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly AppDbContext _context;

    public NotificationService(ILogger<NotificationService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void NotifyUserJoined(int userId, Guid queueId, int waitTimeMinutes)
    {
        string msg = $"You joined a new queue! Your estimated wait time is {waitTimeMinutes} minutes.";
        _logger.LogInformation(msg);

        var notification = new Notification { UserId = userId, Message = msg, Timestamp = DateTime.UtcNow, Status = "sent" };
        _context.Notifications.Add(notification);
        _context.SaveChanges();
    }

    public void NotifyUserAlmostReady(int userId, Guid queueId)
    {
        string msg = $"Your turn is almost here! If necessary, please have any required documents ready.";
        _logger.LogInformation(msg);

        var notification = new Notification { UserId = userId, Message = msg, Timestamp = DateTime.UtcNow, Status = "sent" };
        _context.Notifications.Add(notification);
        _context.SaveChanges();
    }

    public List<NotificationResponse> GetUserNotifications(int userId)
    {
        return _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Timestamp)
            .Select(n => new NotificationResponse { Message = n.Message, Timestamp = n.Timestamp })
            .ToList();
    }
}