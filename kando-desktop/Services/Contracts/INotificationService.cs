using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kando_desktop.DTOs.Responses;
using kando_desktop.Models;

namespace kando_desktop.Services.Contracts
{
    public interface INotificationService
    {
        event Action<string, bool> OnShowNotification;
        ObservableCollection<NotificationItem> RealTimeNotifications { get; }
        void Show(string message, bool isError = false);
        Task InitializeSignalRAsync(string token, int userId);
        Task<List<NotificationResponseDto>> GetMyNotificationsAsync();
        Task LoadHistoricalNotificationsAsync();
        Task DisconnectSignalRAsync();
        void ClearNotifications();
        bool IsConnected { get; }
    }
}
