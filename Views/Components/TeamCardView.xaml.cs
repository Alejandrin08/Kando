using CommunityToolkit.Maui.Views;
using kando_desktop.Helpers;
using kando_desktop.Models;
using kando_desktop.ViewModels.ContentPages;
using kando_desktop.ViewModels.Popups;
using kando_desktop.Views.Popups;
using kando_desktop.Services.Contracts;

namespace kando_desktop.Views.Components
{
    public partial class TeamCardView : ContentView
    {
        public TeamCardView()
        {
            InitializeComponent();
        }

        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            ThreeDotsLabel.Opacity = 1;
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            ThreeDotsLabel.Opacity = 0;
        }

        private void OnOptionsTapped(object sender, EventArgs e)
        {
            var anchor = sender as View;
            var team = this.BindingContext as Team;

            if (team != null)
            {
                var viewModel = new TeamMenuPopupViewModel(team);
                var popup = new TeamMenuPopup();
                popup.BindingContext = viewModel;
                popup.Anchor = anchor;

                viewModel.RequestClose = () => popup.Close();

                viewModel.RequestEditTeam = (selectedTeam) =>
                {
                    ShowModifyTeamPopup(selectedTeam);
                };

                viewModel.RequestRemoveMember = (selectedTeam) =>
                {
                    ShowRemoveMemberPopup(selectedTeam);
                };

                Page currentPage = Shell.Current.CurrentPage ?? App.Current.MainPage;
                currentPage.ShowPopup(popup);
            }
        }

        private void ShowModifyTeamPopup(Team team)
        {
            var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            var notificationService = ServiceHelper.GetService<INotificationService>();

            if (workspaceService != null && notificationService != null)
            {
                var viewModel = new ModifyTeamPopupViewModel(
                    team,
                    workspaceService,
                    notificationService);

                var popup = new ModifyTeamPopup();
                popup.BindingContext = viewModel;

                viewModel.RequestClose = () => popup.Close();

                Page currentPage = Shell.Current.CurrentPage ?? App.Current.MainPage;
                currentPage.ShowPopup(popup);
            }
        }

        private void ShowRemoveMemberPopup(Team team)
        {
            var member = team.Members?.FirstOrDefault();

            var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            var notificationService = ServiceHelper.GetService<INotificationService>();

            if (workspaceService != null && notificationService != null && member != null)
            {
                var viewModel = new RemoveMemberPopupViewModel(
                    team,
                    member,
                    workspaceService,
                    notificationService);

                var popup = new RemoveMemberPopup();
                popup.BindingContext = viewModel;

                viewModel.RequestClose = () => popup.Close();

                Page currentPage = Shell.Current.CurrentPage ?? App.Current.MainPage;
                currentPage.ShowPopup(popup);
            }
        }
    }
}