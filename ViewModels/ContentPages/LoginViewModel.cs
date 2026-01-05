using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.ContentPages
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EyeIconSource))]
        private bool isPasswordHidden = true;

        public string EyeIconSource => IsPasswordHidden ? "show.png" : "hide.png";

        public LoginViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [RelayCommand]
        private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

        [RelayCommand]
        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }

        [RelayCommand]
        private async Task GoToHome()
        {
            await Shell.Current.GoToAsync("HomePage");

            var message = AppResources.WelcomeBack;
            _notificationService.Show(message);
        }
    }
}