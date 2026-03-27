using System.Collections.Generic;

namespace QueueSmart.Api.Services;

public interface INotificationService
{
    void NotifyUserJoined(int userId, int serviceId, int waitTimeMinutes);
    void NotifyUserAlmostReady(int userId, int serviceId);
    List<string> GetUserNotifications(int userId);
}