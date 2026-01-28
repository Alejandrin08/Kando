using kando_desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Contracts
{
    public interface ISessionService
    {
        UserSession? CurrentUser { get; }
        bool IsAuthenticated { get; }
        Task SaveSessionAsync(UserSession user, string token);
        Task LoadSessionAsync();
        void Logout();
    }
}
