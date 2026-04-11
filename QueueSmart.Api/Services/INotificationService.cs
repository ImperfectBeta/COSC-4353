using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueSmart.Api.Services;

public interface INotificationService
{
    Task NotifyUserJoined(int userId, Guid serviceId, int waitTimeMinutes);
    Task NotifyUserAlmostReady(int userId, Guid serviceId);
    Task<List<string>> GetUserNotifications(int userId);
}