using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.Popups
{
    public partial class DeleteTeamPopupViewModel : ObservableObject
    {

        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly ITeamService _teamService;
        private readonly Team _teamToDelete;

        [ObservableProperty]
        private bool isBusy;

        public Action RequestClose;

        public DeleteTeamPopupViewModel(IWorkspaceService workspaceService, INotificationService notificationService, ITeamService teamService, Team teamToDelete)
        {
            _workspaceService = workspaceService;
            _notificationService = notificationService;
            _teamService = teamService;
            _teamToDelete = teamToDelete;
        }

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke();

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task DeleteTeam()
        {
            IsBusy = true;
            try
            {
                var success = await _teamService.DeleteTeamAsync(_teamToDelete.Id);

                if (success)
                {
                    _workspaceService.DeleteTeam(_teamToDelete);
                    RequestClose?.Invoke();
                    _notificationService.Show(AppResources.TeamDeleteSuccessfully);
                }
                else
                {
                    _notificationService.Show(AppResources.FailedToDeleteTeam, true);
                }

            }
            catch (Exception)
            {
                _notificationService.Show(AppResources.UnexpectedError);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
