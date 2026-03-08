using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.Resources.Strings;
using System.Collections.ObjectModel;
using kando_desktop.Enums;

namespace kando_desktop.ViewModels.Popups
{
    public partial class RemoveMemberPopupViewModel : ObservableObject
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly ITeamMemberService _teamMemberService;

        [ObservableProperty]
        private Team teamSelected;

        [ObservableProperty]
        private Member memberSelected;

        public ObservableCollection<Member> RemovableMembers { get; } = new();

        public Action RequestClose;

        [ObservableProperty]
        private bool isBusy;

        public RemoveMemberPopupViewModel(
                Team team,
                IWorkspaceService workspaceService,
                INotificationService notificationService,
                ITeamMemberService teamMemberService)
        {
            TeamSelected = team;
            _workspaceService = workspaceService;
            _notificationService = notificationService;
            _teamMemberService = teamMemberService;

            _ = LoadMembersAsync();
        }

        private async Task LoadMembersAsync()
        {
            IsBusy = true;
            var members = await _teamMemberService.GetMembersAsync(TeamSelected.Id);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var tempMembers = new List<Member>();
                if (members != null)
                {
                    foreach (var m in members)
                    {
                        string displayName = m.Name;

                        var role = m.Status == "Pending" ? TeamRole.Pending : TeamRole.Member;

                        tempMembers.Add(new Member
                        {
                            UserId = m.UserId,
                            Name = displayName,
                            Role = role,
                            Initials = GetInitials(m.Name),
                            BaseColor = TeamSelected.TeamColor
                        });
                    }
                    MemberSelected = RemovableMembers.FirstOrDefault();
                }

                RemovableMembers.Clear();
                foreach (var m in tempMembers) RemovableMembers.Add(m);

                MemberSelected = RemovableMembers.FirstOrDefault();
            });
            IsBusy = false;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Confirm(Member memberToRemove)
        {
            if (TeamSelected != null && memberToRemove != null)
            {
                IsBusy = true;

                string deletedName = memberToRemove.Name;

                bool success = await _teamMemberService.RemoveMemberAsync(TeamSelected.Id, memberToRemove.UserId);

                if (success)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        RemovableMembers.Remove(memberToRemove);
                        MemberSelected = RemovableMembers.FirstOrDefault();
                    });

                    await _workspaceService.ForceRefreshAsync();

                    var message = $"{deletedName} {AppResources.MemberRemovedFromTeam}";
                    _notificationService.Show(message);
                }
                else
                {
                    _notificationService.Show(AppResources.UnexpectedError, true);
                }

                IsBusy = false;
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

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "YO";
            var parts = name.Trim().Split(' ');
            if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
    }
}