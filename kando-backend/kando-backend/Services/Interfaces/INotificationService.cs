using kando_backend.DTOs.Responses;

namespace kando_backend.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationToUserAsync(int userId, object notificationPayload);

        Task<List<NotificationResponseDto>> GetUserNotificationsAsync(int userId);
    }
}