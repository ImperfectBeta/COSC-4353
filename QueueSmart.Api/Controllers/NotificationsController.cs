using Microsoft.AspNetCore.Mvc;
using QueueSmart.Api.Services;

namespace QueueSmart.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // base route is /api/notifications
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("{userId}")] // GET /api/notifications/{userId}
    public ActionResult<IEnumerable<string>> GetNotifications(int userId)
    {
        var notifications = _notificationService.GetUserNotifications(userId);
        
        return Ok(notifications);
    }
}