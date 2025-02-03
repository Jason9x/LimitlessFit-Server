using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Dtos;
using LimitlessFit.Services.Hubs;

namespace LimitlessFit.Services;

public class NotificationService(
    IAuthService authService,
    ApplicationDbContext context,
    IHubContext<NotificationHub> hubContext) : INotificationService
{
    public async Task CreateNotificationAsync(int userId, string messageKey,
        Dictionary<string, object>? additionalData = null)
    {
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

    public async Task<List<NotificationDto>> GetNotificationsAsync()
    {
        var userId = authService.GetUserIdFromClaims();

        return await context.Notifications
            .AsNoTracking()
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAt)
            .Select(notification => new NotificationDto(
                notification.Id,
                notification.MessageKey ?? string.Empty,
                notification.CreatedAt,
                notification.IsRead,
                notification.AdditionalData))
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