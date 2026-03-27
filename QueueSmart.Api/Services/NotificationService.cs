using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace QueueSmart.Api.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private static readonly List<string> _allLogs = new();

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public void NotifyUserJoined(int userId, int serviceId, int waitTimeMinutes)
    {
        string msg = $"User {userId}: You joined the queue for Service {serviceId}. Wait time: {waitTimeMinutes} mins.";
        _logger.LogInformation(msg);
        _allLogs.Add(msg);
    }

    public void NotifyUserAlmostReady(int userId, int serviceId)
    {
        string msg = $"User {userId}: You are almost ready for Service {serviceId}!";
        _logger.LogInformation(msg);
        _allLogs.Add(msg);
    }

    public List<string> GetUserNotifications(int userId)
    {
        return _allLogs.Where(log => log.StartsWith($"User {userId}:")).ToList();
    }
}