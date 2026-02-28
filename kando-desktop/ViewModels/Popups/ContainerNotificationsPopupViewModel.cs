using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.ViewModels.Popups
{
    public partial class ContainerNotificationsPopupViewModel : ObservableObject
    {

        public ObservableCollection<NotificationItem> Notifications { get; }

        public Action RequestClose;

        public ContainerNotificationsPopupViewModel(INotificationService notificationService)
        {
            Notifications = notificationService.RealTimeNotifications;
        }

        [RelayCommand]
        private void Close() => RequestClose?.Invoke();
    }
}
