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
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;

        private const string NamePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$";
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string userName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string email;

        [ObservableProperty]
        private string userInitials;

        [ObservableProperty]
        private Color selectedProfileColor = Color.FromArgb("#8f45ef");

        [ObservableProperty]
        private bool hasNameError;

        [ObservableProperty]
        private string nameErrorText;

        [ObservableProperty]
        private bool hasEmailError;

        [ObservableProperty]
        private string emailErrorText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private bool isBusy;

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

        public ModifyProfileViewModel(INotificationService notificationService, ISessionService sessionService, IUserService userService) : base(sessionService)
        {
            _notificationService = notificationService;
            _sessionService = sessionService;
            _userService = userService;

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

                UpdateInitials(UserName);
            }
        }

        partial void OnUserNameChanged(string value)
        {
            UpdateInitials(value);
            if (HasNameError) { HasNameError = false; NameErrorText = string.Empty; }
        }

        partial void OnEmailChanged(string value)
        {
            if (HasEmailError) { HasEmailError = false; EmailErrorText = string.Empty; }
        }

        private void UpdateInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                UserInitials = "U";
                return;
            }

            var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                UserInitials = parts[0][0].ToString().ToUpper();
            }
            else if (parts.Length >= 2)
            {
                UserInitials = $"{parts[0][0]}{parts[1][0]}".ToUpper();
            }
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
            foreach (var item in Colors)
            {
                item.IsSelected = false;
            }

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
        private async Task BackHome()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}