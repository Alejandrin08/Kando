using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        public event Action<string, bool> OnShowNotification;

        public void Show(string message, bool isError = false)
        {
            OnShowNotification?.Invoke(message, isError);
        }
    }
}
