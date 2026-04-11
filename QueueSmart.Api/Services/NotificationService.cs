using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task NotifyUserJoined(int userId, Guid serviceId, int waitTimeMinutes)
    {
        string msg = $"User {userId}: You joined the queue for Service {serviceId}. Wait time: {waitTimeMinutes} mins.";
        _logger.LogInformation(msg);

        var notification = new Notification 
        { 
            UserId = userId, 
            Message = msg, 
            Timestamp = DateTime.UtcNow, 
            Status = "sent" 
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task NotifyUserAlmostReady(int userId, Guid serviceId)
    {
        string msg = $"User {userId}: You are almost ready for Service {serviceId}!";
        _logger.LogInformation(msg);

        var notification = new Notification 
        { 
            UserId = userId, 
            Message = msg, 
            Timestamp = DateTime.UtcNow, 
            Status = "sent" 
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> GetUserNotifications(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Timestamp)
            .Select(n => n.Message)
            .ToListAsync();
    }
}