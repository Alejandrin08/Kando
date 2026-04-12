using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.DTOs.Requests;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.ContentPages
{
    public partial class ModifyProfileViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

        private const string NamePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$";
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const string PassPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{6,}$";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string userName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string email;

        [ObservableProperty]
        private Color selectedProfileColor = Color.FromArgb("#8f45ef");

        [ObservableProperty] private bool hasNameError;
        [ObservableProperty] private string nameErrorText;

        [ObservableProperty] private bool hasEmailError;
        [ObservableProperty] private string emailErrorText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSavePassword))]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSavePassword))]
        private string newPassword = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSavePassword))]
        private string confirmPassword = string.Empty;

        [ObservableProperty] private bool hasCurrentPasswordError;
        [ObservableProperty] private string currentPasswordErrorText;

        [ObservableProperty] private bool hasNewPasswordError;
        [ObservableProperty] private string newPasswordErrorText;

        [ObservableProperty] private bool hasConfirmPasswordError;
        [ObservableProperty] private string confirmPasswordErrorText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        [NotifyPropertyChangedFor(nameof(CanSavePassword))]
        private bool isBusy;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(CurrentEyeIconSource))] private bool isCurrentPasswordHidden = true;
        public string CurrentEyeIconSource => IsCurrentPasswordHidden ? "show.png" : "hide.png";
        [RelayCommand] private void ToggleCurrentPasswordVisibility() => IsCurrentPasswordHidden = !IsCurrentPasswordHidden;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(NewEyeIconSource))] private bool isNewPasswordHidden = true;
        public string NewEyeIconSource => IsNewPasswordHidden ? "show.png" : "hide.png";
        [RelayCommand] private void ToggleNewPasswordVisibility() => IsNewPasswordHidden = !IsNewPasswordHidden;

        [ObservableProperty][NotifyPropertyChangedFor(nameof(ConfirmEyeIconSource))] private bool isConfirmPasswordHidden = true;
        public string ConfirmEyeIconSource => IsConfirmPasswordHidden ? "show.png" : "hide.png";
        [RelayCommand] private void ToggleConfirmPasswordVisibility() => IsConfirmPasswordHidden = !IsConfirmPasswordHidden;

        public ObservableCollection<ColorItem> Colors { get; } = new()
        {
            new ColorItem { ColorHex = "#8f45ef" },
            new ColorItem { ColorHex = "#e7336a" },
            new ColorItem { ColorHex = "#158ddf" },
            new ColorItem { ColorHex = "#ee770b" },
            new ColorItem { ColorHex = "#13ad64" },
            new ColorItem { ColorHex = "#e42647" },
            new ColorItem { ColorHex = "#4465ee" },
            new ColorItem { ColorHex = "#0da0ae" }
        };

        public bool CanSave => !IsBusy &&
                               !string.IsNullOrWhiteSpace(UserName) && Regex.IsMatch(UserName, NamePattern) &&
                               !string.IsNullOrWhiteSpace(Email) && Regex.IsMatch(Email, EmailPattern);

        public bool CanSavePassword => !IsBusy &&
                                       !string.IsNullOrWhiteSpace(CurrentPassword) &&
                                       !string.IsNullOrWhiteSpace(NewPassword) &&
                                       !string.IsNullOrWhiteSpace(ConfirmPassword);

        public ModifyProfileViewModel(INotificationService notificationService, ISessionService sessionService, IUserService userService) : base(sessionService)
        {
            _notificationService = notificationService;
            _userService = userService;
            _sessionService = sessionService;

            InitializeUserData();
        }

        private void InitializeUserData()
        {
            var currentUser = _sessionService.CurrentUser;
            if (currentUser != null)
            {
                UserName = currentUser.UserName ?? string.Empty;
                Email = currentUser.Email ?? string.Empty;

                string userColorHex = currentUser.UserIcon ?? "#8f45ef";

                var matchedColor = Colors.FirstOrDefault(c => c.ColorHex.Equals(userColorHex, StringComparison.OrdinalIgnoreCase));
                if (matchedColor != null)
                {
                    SelectColor(matchedColor);
                }
                else
                {
                    SelectedProfileColor = Color.FromArgb(userColorHex);
                }
            }
        }

        partial void OnUserNameChanged(string value)
        {
            if (HasNameError) { HasNameError = false; NameErrorText = string.Empty; }
        }

        partial void OnEmailChanged(string value)
        {
            if (HasEmailError) { HasEmailError = false; EmailErrorText = string.Empty; }
        }

        [RelayCommand]
        private void ValidateName()
        {
            bool isValid = !string.IsNullOrWhiteSpace(UserName) && Regex.IsMatch(UserName, NamePattern);
            HasNameError = !isValid;
            NameErrorText = isValid ? string.Empty : (string.IsNullOrWhiteSpace(UserName) ? AppResources.NameRequired : AppResources.InvalidName);
            OnPropertyChanged(nameof(CanSave));
        }

        [RelayCommand]
        private void ValidateEmail()
        {
            bool isValid = !string.IsNullOrWhiteSpace(Email) && Regex.IsMatch(Email, EmailPattern);
            HasEmailError = !isValid;
            EmailErrorText = isValid ? string.Empty : (string.IsNullOrWhiteSpace(Email) ? AppResources.EmailRequired : AppResources.InvalidEmail);
            OnPropertyChanged(nameof(CanSave));
        }

        [RelayCommand]
        private void SelectColor(ColorItem colorItem)
        {
            foreach (var item in Colors) item.IsSelected = false;
            colorItem.IsSelected = true;
            SelectedProfileColor = Color.FromArgb(colorItem.ColorHex);
        }

        [RelayCommand(CanExecute = nameof(CanSave), AllowConcurrentExecutions = false)]
        private async Task Save()
        {
            if (IsBusy) return;

            ValidateName();
            ValidateEmail();
            if (HasNameError || HasEmailError) return;

            IsBusy = true;
            var newColorHex = Colors.FirstOrDefault(c => c.IsSelected)?.ColorHex ?? "#8f45ef";

            var userToUpdate = new UpdateUserDto
            {
                UserName = UserName.Trim(),
                UserEmail = Email.Trim(),
                UserIcon = newColorHex
            };

            try
            {
                var success = await _userService.UpdateUserAsync(userToUpdate);

                if (success)
                {
                    await _sessionService.UpdateSessionDataAsync(UserName, Email, newColorHex);
                    _notificationService.Show(AppResources.ProfileUpdatedSuccessfully);
                    await BackHome();
                }
                else
                {
                    _notificationService.Show(AppResources.ErrorUpdatingProfile, true);
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    HasEmailError = true;
                    EmailErrorText = AppResources.EmailAlreadyRegistered;
                    OnPropertyChanged(nameof(CanSave));
                }
                else
                {
                    _notificationService.Show(AppResources.NetworkError, true);
                }
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
        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                HasCurrentPasswordError = true;
                CurrentPasswordErrorText = AppResources.CurrentPasswordRequired;
            }
            else HasCurrentPasswordError = false;

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

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                HasConfirmPasswordError = true;
                ConfirmPasswordErrorText = AppResources.ConfirmPasswordRequired;
            }
            else if (NewPassword != ConfirmPassword)
            {
                HasConfirmPasswordError = true;
                ConfirmPasswordErrorText = AppResources.PasswordsDoNotMatch;
            }
            else HasConfirmPasswordError = false;
        }

        [RelayCommand(CanExecute = nameof(CanSavePassword), AllowConcurrentExecutions = false)]
        private async Task SavePassword()
        {
            if (IsBusy) return;

            ValidatePassword();

            if (HasCurrentPasswordError || HasNewPasswordError || HasConfirmPasswordError)
                return;

            IsBusy = true;

            try
            {
                var dto = new UpdatePasswordDto
                {
                    CurrentPassword = CurrentPassword.Trim(),
                    NewPassword = NewPassword.Trim(),
                    ConfirmPassword = ConfirmPassword.Trim()
                };

                var success = await _userService.UpdateUserPasswordAsync(dto);

                if (success)
                {
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;

                    _notificationService.Show(AppResources.PasswordUpdatedSuccessfully);

                    await Task.Delay(1500);
                    _sessionService.Logout();
                    await Shell.Current.Navigation.PopToRootAsync(false);
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    _notificationService.Show(AppResources.ErrorUpdatingPassword, true);
                }
            }
            catch (HttpRequestException ex)
            {
                _notificationService.Show(ex.Message, true);

                if (ex.StatusCode == HttpStatusCode.BadRequest || ex.StatusCode == HttpStatusCode.Conflict)
                {
                    if (ex.Message == AppResources.WrongCurrentPassword)
                    {
                        HasCurrentPasswordError = true;
                        CurrentPasswordErrorText = ex.Message;
                    }
                    else if (ex.Message == AppResources.SamePasswordError)
                    {
                        HasNewPasswordError = true;
                        NewPasswordErrorText = ex.Message;
                    }
                    else if (ex.Message == AppResources.PasswordsDoNotMatch)
                    {
                        HasConfirmPasswordError = true;
                        ConfirmPasswordErrorText = ex.Message;
                    }
                }
                else
                {
                    _notificationService.Show(AppResources.NetworkError, true);
                }
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
        private async Task BackHome()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}