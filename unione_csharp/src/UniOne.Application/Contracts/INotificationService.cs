using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetNotifications(long userId);
    Task MarkAsRead(Guid notificationId, long userId);
    Task MarkAllAsRead(long userId);
    Task DeleteNotification(Guid notificationId, long userId);
}
