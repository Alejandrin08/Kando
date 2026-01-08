using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.Resources.Strings;

namespace kando_desktop.ViewModels.Popups
{
    public partial class RemoveMemberPopupViewModel : ObservableObject
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private Team teamSelected;

        [ObservableProperty]
        private Member memberSelected;

        public Action RequestClose;

        public RemoveMemberPopupViewModel(
            Team team,
            Member member,
            IWorkspaceService workspaceService,
            INotificationService notificationService)
        {
            TeamSelected = team;
            MemberSelected = member;
            _workspaceService = workspaceService;
            _notificationService = notificationService;
        }

        [RelayCommand]
        private void Confirm(Member memberToRemove)
        {
            if (TeamSelected != null && memberToRemove != null)
            {
                _workspaceService.DeleteMemberTeam(memberToRemove, TeamSelected);

                var message = $"{memberToRemove.Name} {AppResources.MemberRemovedFromTeam}"; 
                _notificationService.Show(message);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke();
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke();
        }
    }
}