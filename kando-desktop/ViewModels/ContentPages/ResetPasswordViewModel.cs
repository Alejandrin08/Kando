using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.DTOs.Requests;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.ContentPages
{
    [QueryProperty(nameof(Email), "UserEmail")]
    [QueryProperty(nameof(Code), "ValidCode")]
    public partial class ResetPasswordViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;
        private readonly ISessionService _sessionService;

        private const string PassPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{6,}$";

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string code;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanReset))]
        private string newPassword = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanReset))]
        private string confirmNewPassword = string.Empty;

        [ObservableProperty] private bool hasNewPasswordError;
        [ObservableProperty] private string newPasswordErrorText;

        [ObservableProperty] private bool hasConfirmNewPasswordError;
        [ObservableProperty] private string confirmNewPasswordErrorText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanReset))]
        private bool isBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewEyeIconSource))]
        private bool isNewPasswordHidden = true;

        public string NewEyeIconSource => IsNewPasswordHidden ? "show.png" : "hide.png";

        [RelayCommand]
        private void ToggleNewPasswordVisibility() => IsNewPasswordHidden = !IsNewPasswordHidden;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConfirmNewEyeIconSource))]
        private bool isConfirmNewPasswordHidden = true;

        public string ConfirmNewEyeIconSource => IsConfirmNewPasswordHidden ? "show.png" : "hide.png";

        [RelayCommand]
        private void ToggleConfirmNewPasswordVisibility() => IsConfirmNewPasswordHidden = !IsConfirmNewPasswordHidden;

        public bool CanReset => !IsBusy &&
                                !string.IsNullOrWhiteSpace(NewPassword) &&
                                Regex.IsMatch(NewPassword, PassPattern) &&
                                !string.IsNullOrWhiteSpace(ConfirmNewPassword) &&
                                NewPassword == ConfirmNewPassword;

        public ResetPasswordViewModel(IAuthService authService, INotificationService notificationService, ISessionService sessionService) : base(sessionService)
        {
            _authService = authService;
            _notificationService = notificationService;
            _sessionService = sessionService;
        }

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                HasNewPasswordError = true;
                NewPasswordErrorText = AppResources.NewPasswordRequired;
            }
            else if (!Regex.IsMatch(NewPassword, PassPattern))
            {
                HasNewPasswordError = true;
                NewPasswordErrorText = AppResources.InvalidPasswordFormat;
            }
            else HasNewPasswordError = false;

            if (string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                HasConfirmNewPasswordError = true;
                ConfirmNewPasswordErrorText = AppResources.ConfirmPasswordRequired;
            }
            else if (NewPassword != ConfirmNewPassword)
            {
                HasConfirmNewPasswordError = true;
                ConfirmNewPasswordErrorText = AppResources.PasswordsDoNotMatch;
            }
            else HasConfirmNewPasswordError = false;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task ResetPassword()
        {
            ValidatePassword();
            if (HasNewPasswordError || HasConfirmNewPasswordError) return;

            if (IsBusy) return;
            IsBusy = true;

            var resetDto = new ResetPasswordDto
            {
                Email = this.Email,
                Code = this.Code,
                NewPassword = this.NewPassword,
                ConfirmPassword = this.ConfirmNewPassword
            };

            try
            {
                var success = await _authService.ResetPasswordAsync(resetDto);

                if (success)
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    await Task.Delay(200);
                    _notificationService.Show(AppResources.PasswordUpdatedSuccessfully);
                }
                else
                {
                    _notificationService.Show(AppResources.ErrorUpdatingPassword, true);
                }
            }
            catch (HttpRequestException ex)
            {
                _notificationService.Show(AppResources.NetworkError, true);
            }
            catch (Exception)
            {
                _notificationService.Show(AppResources.UnexpectedError, true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnNewPasswordChanged(string value)
        {
            if (HasNewPasswordError && Regex.IsMatch(value, PassPattern))
            {
                HasNewPasswordError = false;
            }

            if (HasConfirmNewPasswordError && value == ConfirmNewPassword)
            {
                HasConfirmNewPasswordError = false;
            }
        }

        partial void OnConfirmNewPasswordChanged(string value)
        {
            if (HasConfirmNewPasswordError && value == NewPassword)
            {
                HasConfirmNewPasswordError = false;
            }
        }
    }
}