using Microsoft.AspNetCore.Mvc;
using MovieApi.Utilities;

namespace MovieApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationHandler notificationHandler;

    public NotificationsController(NotificationHandler notificationHandler)
    {
        this.notificationHandler = notificationHandler;
    }
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetNotifications()
    {
        await this.notificationHandler.HandleAsync("Notification message");
        return Ok("Notifications sent.");
    }
}
