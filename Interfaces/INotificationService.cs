using LimitlessFit.Models;
using LimitlessFit.Models.Dtos;

namespace LimitlessFit.Interfaces;

public interface INotificationService
{
    Task CreateNotificationAsync(int userId, string messageKey, Dictionary<string, object>? additionalData = null);
    Task<List<NotificationDto>> GetNotificationsAsync();
    Task MarkNotificationAsReadAsync(int id);
    Task DeleteNotificationsAsync();
}