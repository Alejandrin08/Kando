using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using System.Text.RegularExpressions;

namespace kando_desktop.ViewModels.ContentPages
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;


        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EyeIconSource))]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private bool hasEmailError;

        [ObservableProperty]
        private string emailErrorText = string.Empty;

        [ObservableProperty]
        private bool hasPasswordError;

        [ObservableProperty]
        private string passwordErrorText = string.Empty;

        [ObservableProperty]
        private bool hasLoginError;

        [ObservableProperty]
        private string loginErrorMessage = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanLogin))]
        private bool isEmailValid;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanLogin))]
        private bool isPasswordValid;

        public bool CanLogin => IsEmailValid && IsPasswordValid && !IsBusy;

        public string EyeIconSource => IsPasswordHidden ? "show.png" : "hide.png";

        public LoginViewModel(INotificationService notificationService, IAuthService authService, ISessionService sessionService) : base(sessionService)
        {
            _notificationService = notificationService;
            _authService = authService;
            _sessionService = sessionService;
        }


        [RelayCommand]
        private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

        [RelayCommand]
        private async Task GoToRegister() => await Shell.Current.GoToAsync("RegisterPage");

        [RelayCommand]
        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                HasEmailError = true;
                EmailErrorText = AppResources.FieldRequired;
                IsEmailValid = false;
            }
            else if (!IsValidEmail(Email))
            {
                HasEmailError = true;
                EmailErrorText = AppResources.InvalidEmail;
                IsEmailValid = false;
            }
            else
            {
                HasEmailError = false;
                EmailErrorText = string.Empty;
                IsEmailValid = true;
            }
        }

        [RelayCommand]
        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                HasPasswordError = true;
                PasswordErrorText = AppResources.FieldRequired;
                IsPasswordValid = false;
            }
            else
            {
                HasPasswordError = false;
                PasswordErrorText = string.Empty;
                IsPasswordValid = true;
            }
        }

        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;

            HasLoginError = false;
            LoginErrorMessage = string.Empty;

            ValidateEmail();
            ValidatePassword();

            if (!IsEmailValid || !IsPasswordValid) return;

            IsBusy = true;

            try
            {
                var response = await _authService.LoginAsync(Email, Password);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    var session = new UserSession
                    {
                        UserId = response.Id,
                        UserName = response.Username,
                        Email = response.Email,
                        UserIcon = response.UserIcon
                    };

                    await _sessionService.SaveSessionAsync(session, response.Token);

                    await Shell.Current.GoToAsync("//HomePage");

                    var message = AppResources.WelcomeBack + " " + response.Username;
                    _notificationService.Show(message);
                }
                else
                {
                    ShowLoginError(AppResources.InvalidCredentials);
                }
            }
            catch (Exception)
            {
                ShowLoginError(AppResources.NetworkError);
            }
            finally
            {
                IsBusy = false;
            }
        }


        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private void ShowLoginError(string message)
        {
            HasLoginError = true;
            LoginErrorMessage = message;

            HasEmailError = true;
            EmailErrorText = string.Empty;
            HasPasswordError = true;
            PasswordErrorText = string.Empty;

            IsEmailValid = IsValidEmail(Email);
            IsPasswordValid = !string.IsNullOrWhiteSpace(Password);
        }

        partial void OnEmailChanged(string value)
        {
            if (HasEmailError && !string.IsNullOrEmpty(value))
            {
                HasEmailError = false;
                EmailErrorText = string.Empty;
            }

            if (HasLoginError)
            {
                HasLoginError = false;
                LoginErrorMessage = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                IsEmailValid = IsValidEmail(value);
            }
            else
            {
                IsEmailValid = false;
            }
        }

        partial void OnPasswordChanged(string value)
        {
            if (HasPasswordError && !string.IsNullOrEmpty(value))
            {
                HasPasswordError = false;
                PasswordErrorText = string.Empty;
            }

            if (HasLoginError)
            {
                HasLoginError = false;
                LoginErrorMessage = string.Empty;
            }

            IsPasswordValid = !string.IsNullOrWhiteSpace(value);
        }
    }
}