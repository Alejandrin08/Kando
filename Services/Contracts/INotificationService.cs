using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Contracts
{
    public interface INotificationService
    {
        event Action<string, bool> OnShowNotification;
        void Show(string message, bool isError = false);
    }
}
