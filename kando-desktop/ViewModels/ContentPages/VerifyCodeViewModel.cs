using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.DTOs.Requests;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.ContentPages
{
    [QueryProperty(nameof(Email), "UserEmail")]
    public partial class VerifyCodeViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSend))]
        private string verificationCode = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSend))]
        private bool isBusy;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ResendCommand))]
        private bool canResend;

        [ObservableProperty]
        private string resendText = AppResources.Resend;

        public bool CanSend => VerificationCode?.Length == 6;

        public VerifyCodeViewModel(IAuthService authService, INotificationService notificationService, ISessionService sessionService) : base(sessionService)
        {
            _authService = authService;
            _notificationService = notificationService;
            _ = StartCountdownAsync(60);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Verify()
        {
            if (IsBusy) return;
            IsBusy = true;

            var code = new ValidateCodeDto
            {
                Email = Email.Trim(),
                Code = VerificationCode
            };

            try
            {
                var success = await _authService.ValidateRecoveryCodeAsync(code);

                if (success)
                {
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "UserEmail", Email },
                        { "ValidCode", VerificationCode }
                    };
                    await Shell.Current.GoToAsync("ResetPasswordPage", navigationParameter);
                }
                else
                {
                    _notificationService.Show(AppResources.ErrorValidatingCode, true);
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

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanResend))]
        private async Task Resend()
        {
            if (IsBusy) return;
            IsBusy = true;

            var emailDto = new ForgotPasswordDto
            {
                Email = Email.Trim()
            };

            try
            {
                var success = await _authService.GenerateRecoveryCodeAsync(emailDto);

                if (success)
                {
                    await Task.Delay(150);
                    _notificationService.Show(AppResources.CodeSentSuccessfully);

                    _ = StartCountdownAsync(60);
                }
                else
                {
                    _notificationService.Show(AppResources.FailedSentCode, true);
                }
            }
            catch (HttpRequestException)
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
            await Shell.Current.Navigation.PopAsync();
        }

        private async Task StartCountdownAsync(int seconds)
        {
            CanResend = false;

            for (int i = seconds; i > 0; i--)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);
                ResendText = $"{AppResources.ResendIn} {time:m\\:ss}";
                await Task.Delay(1000);
            }

            ResendText = AppResources.Resend;
            CanResend = true;
        }
    }
}