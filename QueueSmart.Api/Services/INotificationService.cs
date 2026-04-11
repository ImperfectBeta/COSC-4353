using System.Collections.Generic;

namespace QueueSmart.Api.Services;

public interface INotificationService
{
    void NotifyUserJoined(int userId, Guid queueId, int waitTimeMinutes);
    void NotifyUserAlmostReady(int userId, Guid queueId);
    List<string> GetUserNotifications(int userId);
}