using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Services.Hubs;
using Newtonsoft.Json;

namespace LimitlessFit.Services;

public class NotificationService(
    IAuthService authService,
    ApplicationDbContext context,
    IHubContext<NotificationHub> hubContext) : INotificationService
{
    public async Task CreateNotificationAsync(string messageKey, Dictionary<string, object>? additionalData = null)
    {
        var userId = authService.GetUserIdFromClaims();
        var notification = new Notification
        {
            UserId = userId,
            MessageKey = messageKey,
            CreatedAt = DateTime.UtcNow,
            AdditionalData = additionalData?.Count > 0
                ? JsonConvert.SerializeObject(additionalData)
                : null
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync();

        await hubContext.Clients
            .User(userId.ToString())
            .SendAsync("NewNotification", notification);
    }

    public async Task<List<Notification>> GetNotificationsAsync()
    {
        var userId = authService.GetUserIdFromClaims();

        return await context.Notifications
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkNotificationAsReadAsync(int id)
    {
        var userId = authService.GetUserIdFromClaims();

        await context.Notifications
            .Where(notification => notification.UserId == userId && notification.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(notification => notification.IsRead, true));
    }

    public async Task DeleteNotificationsAsync()
    {
        var userId = authService.GetUserIdFromClaims();

        await context.Notifications
            .Where(notification => notification.UserId == userId)
            .ExecuteDeleteAsync();
    }
}