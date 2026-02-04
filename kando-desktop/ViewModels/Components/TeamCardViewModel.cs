using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Helpers;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.ViewModels.Popups;
using kando_desktop.Views.Popups;
using InviteFriendPopup = kando_desktop.Views.Popups.InviteFriendPopup;

namespace kando_desktop.ViewModels.Components
{
    public partial class TeamCardViewModel : ObservableObject
    {
        [ObservableProperty]
        private Team _team;

        private readonly ITeamService _teamService;
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;

        public TeamCardViewModel(Team team)
        {
            _team = team;

            _teamService = ServiceHelper.GetService<ITeamService>();
            _workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            _notificationService = ServiceHelper.GetService<INotificationService>();
        }

        [RelayCommand]
        private void OpenInvitePopup()
        {
            if (_teamService == null || _notificationService == null) return;

            var viewModel = new InviteFriendPopupViewModel(Team, _teamService, _notificationService);
            var popup = new InviteFriendPopup { BindingContext = viewModel };

            viewModel.RequestClose = () => popup.Close();

            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        [RelayCommand]
        private void OpenOptions(View anchor)
        {
            var viewModel = new TeamMenuPopupViewModel(Team);
            var popup = new TeamMenuPopup
            {
                BindingContext = viewModel,
                Anchor = anchor
            };

            viewModel.RequestClose = () => popup.Close();

            viewModel.RequestEditTeam = (t) => ShowModifyTeamPopup();
            viewModel.RequestRemoveMember = (t) => ShowRemoveMemberPopup();
            viewModel.RequestDeleteTeam = (t) => ShowDeleteTeamPopup();

            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        private void ShowModifyTeamPopup()
        {
            var viewModel = new ModifyTeamPopupViewModel(Team, _workspaceService, _notificationService, _teamService);
            var popup = new ModifyTeamPopup { BindingContext = viewModel };
            viewModel.RequestClose = () => popup.Close();
            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        private void ShowRemoveMemberPopup()
        {
            var member = Team.Members?.FirstOrDefault();
            if (member == null) return;

            var viewModel = new RemoveMemberPopupViewModel(Team, member, _workspaceService, _notificationService);
            var popup = new RemoveMemberPopup { BindingContext = viewModel };
            viewModel.RequestClose = () => popup.Close();
            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        private void ShowDeleteTeamPopup()
        {
            var viewModel = new DeleteTeamPopupViewModel(_workspaceService, _notificationService, _teamService, Team);
            var popup = new DeleteTeamPopup { BindingContext = viewModel };
            viewModel.RequestClose = () => popup.Close();
            Shell.Current.CurrentPage.ShowPopup(popup);
        }
    }
}