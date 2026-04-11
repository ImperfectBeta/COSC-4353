using System.Collections.Generic;

using QueueSmart.Api.DTOs;

namespace QueueSmart.Api.Services;

public interface INotificationService
{
    void NotifyUserJoined(int userId, Guid queueId, int waitTimeMinutes);
    void NotifyUserAlmostReady(int userId, Guid queueId);
    List<NotificationResponse> GetUserNotifications(int userId);
}