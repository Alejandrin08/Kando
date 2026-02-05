using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kando_desktop.ViewModels.Popups
{
    public partial class InviteFriendPopupViewModel : ObservableObject
    {

        private readonly Team _team;
        private readonly ITeamService _teamService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private bool hasNameError;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string? nameErrorMessage;

        public Action RequestClose;

        public InviteFriendPopupViewModel(Team team, ITeamService teamService, INotificationService notificationService)
        {
            _team = team;
            _teamService = teamService;
            _notificationService = notificationService;
        }

        [RelayCommand]
        private void Close() => RequestClose?.Invoke();


        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task SendEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                HasNameError = true;
                NameErrorMessage = AppResources.FieldRequired;
                return;
            }

            if (!IsValidEmail(Email))
            {
                HasNameError = true;
                NameErrorMessage = AppResources.InvalidEmail;
                return;
            }

            HasNameError = false;
            NameErrorMessage = null;
            IsBusy = true;

            try
            {
                string? errorResult = await _teamService.InviteMemberAsync(_team.Id, Email);

                if (errorResult == null)
                {
                    _notificationService.Show(AppResources.InvitationSentSuccessfully);
                    RequestClose?.Invoke();
                }
                else
                {
                    _notificationService.Show(errorResult, true);
                }
            }
            catch (Exception)
            {
                _notificationService.Show(AppResources.GenericInviteError, true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        partial void OnEmailChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) HasNameError = false;
        }
    }
}
