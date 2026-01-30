using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.Popups
{
    public partial class ProfileMenuPopupViewModel : ObservableObject
    {

        private readonly INotificationService _notificationService;
        private readonly ISessionService _sessionService;
        private readonly IWorkspaceService _workspaceService;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string userEmail;

        public Action RequestClose;

        public ProfileMenuPopupViewModel(string userName, string userEmail, INotificationService notificationService, ISessionService sessionService, IWorkspaceService workspaceService)
        {
            UserName = userName;
            UserEmail = userEmail;
            _notificationService = notificationService;
            _sessionService = sessionService;
            _workspaceService = workspaceService;
        }

        [RelayCommand]
        private async Task Logout()
        {
            RequestClose?.Invoke();

            await Task.Delay(150);
            
            _workspaceService.ClearData();
            _sessionService.Logout();
            await Shell.Current.GoToAsync("//LoginPage");

            var message = AppResources.Logout;
            _notificationService.Show(message);
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke();
        }
    }
}