using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.DTOs.Requests;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.ContentPages
{
    public partial class VerifyEmailViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;

        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSend))]
        private string email;

        [ObservableProperty] private bool hasEmailError;
        [ObservableProperty] private string emailErrorText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSend))]
        private bool isBusy;

        public bool CanSend => !IsBusy &&
                               !string.IsNullOrWhiteSpace(Email) &&
                               Regex.IsMatch(Email.Trim(), EmailPattern);

        public VerifyEmailViewModel(IAuthService authService, INotificationService notificationService, ISessionService sessionService) : base(sessionService)
        {
            _authService = authService;
            _notificationService = notificationService;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Validate()
        {
            if (IsBusy) return;

            ValidateEmail();
            if (HasEmailError) return;

            IsBusy = true;

            var email = new ForgotPasswordDto
            {
                Email = Email.Trim()
            };

            try
            {
                var success = await _authService.GenerateRecoveryCodeAsync(email);

                if (success)
                {
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "UserEmail", Email.Trim() }
                    };

                    await Shell.Current.GoToAsync("VerifyCodePage", navigationParameter);
                    await Task.Delay(150);
                    _notificationService.Show(AppResources.CodeSentSuccessfully);
                }
                else
                {
                    _notificationService.Show(AppResources.FailedSentCode, true);
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

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.Navigation.PopToRootAsync(false);
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        private void ValidateEmail()
        {
            bool isValid = !string.IsNullOrWhiteSpace(Email) && Regex.IsMatch(Email.Trim(), EmailPattern);
            HasEmailError = !isValid;
            EmailErrorText = isValid ? string.Empty : (string.IsNullOrWhiteSpace(Email) ? AppResources.EmailRequired : AppResources.InvalidEmail);
            OnPropertyChanged(nameof(CanSend));
        }

        partial void OnEmailChanged(string value)
        {
            if (HasEmailError) { HasEmailError = false; EmailErrorText = string.Empty; }
        }
    }
}