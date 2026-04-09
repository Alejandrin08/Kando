using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace kando_desktop.Services.Implementations
{
    public class SessionService : ISessionService
    {
        public UserSession? CurrentUser { get; private set; }

        public bool IsAuthenticated => CurrentUser != null;

        private const string SessionKey = "user_session_data";
        private const string TokenKey = "auth_token";
        public event Action OnSessionUpdated;

        public async Task SaveSessionAsync(UserSession user, string token)
        {
            CurrentUser = user;

            await SecureStorage.SetAsync(TokenKey, token);

            string json = JsonSerializer.Serialize(user);
            Preferences.Set(SessionKey, json);
        }

        public async Task LoadSessionAsync()
        {
            var token = await SecureStorage.GetAsync(TokenKey);

            if (Preferences.ContainsKey(SessionKey) && !string.IsNullOrEmpty(token))
            {
                string json = Preferences.Get(SessionKey, string.Empty);
                CurrentUser = JsonSerializer.Deserialize<UserSession>(json);
            }
            else
            {
                CurrentUser = null;
            }
        }

        public async Task UpdateSessionDataAsync(string newName, string newEmail, string newIcon)
        {
            if (CurrentUser != null)
            {
                CurrentUser.UserName = newName;
                CurrentUser.Email = newEmail;
                CurrentUser.UserIcon = newIcon;

                Preferences.Set("user_name", newName);
                Preferences.Set("user_email", newEmail);
                Preferences.Set("user_icon", newIcon);

                OnSessionUpdated?.Invoke();
            }
        }

        public void Logout()
        {
            CurrentUser = null;
            SecureStorage.Remove(TokenKey);
            Preferences.Remove(SessionKey);
        }
    }
}
