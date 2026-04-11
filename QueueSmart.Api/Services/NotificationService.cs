using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        string msg = $"User {userId}: You joined the queue {queueId}. Wait time: {waitTimeMinutes} mins.";
        _logger.LogInformation(msg);

        var notification = new Notification { UserId = userId, Message = msg, Timestamp = DateTime.UtcNow, Status = "sent" };
        _context.Notifications.Add(notification);
        _context.SaveChanges();
    }

    public void NotifyUserAlmostReady(int userId, Guid queueId)
    {
        string msg = $"User {userId}: You are almost ready for queue {queueId}!";
        _logger.LogInformation(msg);

        var notification = new Notification { UserId = userId, Message = msg, Timestamp = DateTime.UtcNow, Status = "sent" };
        _context.Notifications.Add(notification);
        _context.SaveChanges();
    }

    public List<string> GetUserNotifications(int userId)
    {
        return _context.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.Timestamp).Select(n => n.Message).ToList();
    }
}