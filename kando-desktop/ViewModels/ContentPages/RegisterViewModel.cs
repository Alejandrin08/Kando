using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.DTOs.Requests;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using System.Net;
using System.Text.RegularExpressions;

namespace kando_desktop.ViewModels.ContentPages
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        private const string NamePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$";
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const string PassPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{6,}$";

        public RegisterViewModel(IUserService userService, INotificationService notificationService, ISessionService sessionService) : base(sessionService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }
    
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanRegister))] 
        private string userName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanRegister))]
        private string email = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanRegister))]
        private string password = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EyeIconSource))]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        [NotifyPropertyChangedFor(nameof(CanRegister))]
        private bool isBusy;

        [ObservableProperty] bool hasNameError;
        [ObservableProperty] string nameErrorText;

        [ObservableProperty] bool hasEmailError;
        [ObservableProperty] string emailErrorText;

        [ObservableProperty] bool hasPasswordError;
        [ObservableProperty] string passwordErrorText;

        [ObservableProperty] bool hasRegisterError;
        [ObservableProperty] string registerErrorMessage;

        public bool IsNotBusy => !IsBusy;
        public string EyeIconSource => IsPasswordHidden ? "show.png" : "hide.png";

        public bool CanRegister =>
            !string.IsNullOrWhiteSpace(UserName) && !HasNameError &&
            !string.IsNullOrWhiteSpace(Email) && !HasEmailError &&
            !string.IsNullOrWhiteSpace(Password) && !HasPasswordError &&
            !IsBusy;


        [RelayCommand]
        private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

        [RelayCommand]
        private async Task GoToLogin() => await Shell.Current.GoToAsync("//LoginPage");

        [RelayCommand]
        private void ValidateName()
        {
            bool isValid = !string.IsNullOrWhiteSpace(UserName) && Regex.IsMatch(UserName, NamePattern);
            HasNameError = !isValid;
            NameErrorText = isValid ? string.Empty : (string.IsNullOrWhiteSpace(UserName) ? AppResources.NameRequired : AppResources.InvalidName);
            OnPropertyChanged(nameof(CanRegister));
        }

        [RelayCommand]
        private void ValidateEmail()
        {
            bool isValid = !string.IsNullOrWhiteSpace(Email) && Regex.IsMatch(Email, EmailPattern);
            HasEmailError = !isValid;
            EmailErrorText = isValid ? string.Empty : (string.IsNullOrWhiteSpace(Email) ? AppResources.EmailRequired : AppResources.InvalidEmail);
            OnPropertyChanged(nameof(CanRegister));
        }

        [RelayCommand]
        private void ValidatePassword()
        {
            bool isValid = !string.IsNullOrWhiteSpace(Password) && Regex.IsMatch(Password, PassPattern);
            HasPasswordError = !isValid;
            PasswordErrorText = isValid ? string.Empty : (string.IsNullOrWhiteSpace(Password) ? AppResources.PasswordRequired : AppResources.InvalidPassword);
            OnPropertyChanged(nameof(CanRegister));
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Register()
        {
            if (IsBusy) return;

            ValidateName(); ValidateEmail(); ValidatePassword();
            if (HasNameError || HasEmailError || HasPasswordError) return;

            IsBusy = true;
            HasRegisterError = false;

            try
            {
                var success = await _userService.CreateUserAsync(new CreateUserDto
                {
                    UserName = UserName.Trim(),
                    UserEmail = Email.Trim(),
                    Password = Password
                });

                await GoToLogin();

                _notificationService.Show(AppResources.RegistrationSuccess);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict) 
                {
                    HasEmailError = true;
                    EmailErrorText = AppResources.EmailAlreadyRegistered;
                    OnPropertyChanged(nameof(CanRegister));
                }
                else
                {
                    ShowGlobalError(AppResources.NetworkError);
                }
            }
            catch
            {
                ShowGlobalError(AppResources.UnexpectedError);
            }
            finally
            {
                IsBusy = false;
            }
        }


        private void ShowGlobalError(string msg) { HasRegisterError = true; RegisterErrorMessage = msg; }

        partial void OnUserNameChanged(string value)
        {
            if (HasNameError) { HasNameError = false; NameErrorText = ""; }
        }

        partial void OnEmailChanged(string value)
        {
            if (HasEmailError) { HasEmailError = false; EmailErrorText = ""; }
        }

        partial void OnPasswordChanged(string value)
        {
            if (HasPasswordError) { HasPasswordError = false; PasswordErrorText = ""; }
        }
    }
}