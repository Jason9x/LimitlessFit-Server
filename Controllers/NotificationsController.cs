using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<Notification>>> GetNotifications()
    {
        return await notificationService.GetNotificationsAsync();
    }

    [HttpPut("{id:int}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await notificationService.MarkNotificationAsReadAsync(id);
        
        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAllNotifications()
    {
        await notificationService.DeleteNotificationsAsync();
        
        return NoContent();
    }
}