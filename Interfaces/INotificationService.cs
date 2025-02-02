using LimitlessFit.Models;

namespace LimitlessFit.Interfaces;

public interface INotificationService
{
    Task CreateNotificationAsync(int userId, string messageKey, Dictionary<string, object>? additionalData = null);
    Task<List<Notification>> GetNotificationsAsync();
    Task MarkNotificationAsReadAsync(int id);
    Task DeleteNotificationsAsync();
}