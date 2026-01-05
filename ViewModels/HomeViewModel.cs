using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Services;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using System.Collections.ObjectModel;

namespace kando_desktop.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;

        public ObservableCollection<Board> FilteredBoards { get; } = new();
        public ObservableCollection<Team> Teams => _workspaceService.Teams;
        
        public Action RequestShowCreateTeam;
        public Action RequestShowCreateBoard;
        public Action RequestClosePopup;

        [ObservableProperty] private bool isTeamDropdownOpen;
        [ObservableProperty] private Team selectedTeam;
        [ObservableProperty] private string searchText;

        [ObservableProperty] private int activeBoardsCount;
        [ObservableProperty] private int completedTasksCount;
        [ObservableProperty] private int totalTasksCount;
        [ObservableProperty] private int teamsCount;

        public HomeViewModel(IWorkspaceService workspaceService, INotificationService notificationService)
        {
            _workspaceService = workspaceService;
            _notificationService = notificationService;

            RefreshData();
        }

        private void RefreshData()
        {
            PerformSearch(SearchText);
            UpdateStats();
        }

        partial void OnSearchTextChanged(string value)
        {
            PerformSearch(value);
        }

        private void PerformSearch(string query)
        {
            FilteredBoards.Clear();
            var source = _workspaceService.Boards;

            var items = string.IsNullOrWhiteSpace(query)
                ? source
                : source.Where(b => b.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

            foreach (var item in items)
            {
                FilteredBoards.Add(item);
            }
        }

        private void UpdateStats()
        {
            TeamsCount = _workspaceService.Teams.Count;
            ActiveBoardsCount = _workspaceService.Boards.Count;
        }


        public void AddNewTeam(string name, string iconSource, Color teamColor)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            _workspaceService.CreateTeam(name, iconSource, teamColor);

            var message = AppResources.TeamCreatedSuccessfully;

            _notificationService.Show(message);

            UpdateStats();
            OnPropertyChanged(nameof(Teams));
        }

        public void AddNewBoard(string name, string iconSource, Team team)
        {
            if (string.IsNullOrWhiteSpace(name) || team == null) return;

            _workspaceService.CreateBoard(name, iconSource, team);
            
            var message = AppResources.BoardCreatedSuccessfully;

            _notificationService.Show(message);

            RefreshData();
        }

        private void ErrorExample()
        {
            var message = AppResources.ErrorConnectingServer;
            _notificationService.Show(message, true);
        }

        [RelayCommand]
        private void CreateTeam() => RequestShowCreateTeam?.Invoke();

        [RelayCommand]
        private void CreateBoard()
        {
            if (Teams.Count > 0 && SelectedTeam == null)
            {
                SelectedTeam = Teams[0];
            }
            IsTeamDropdownOpen = false;
            RequestShowCreateBoard?.Invoke();
        }

        [RelayCommand]
        private void ToggleTeamDropdown() => IsTeamDropdownOpen = !IsTeamDropdownOpen;

        [RelayCommand]
        private void SelectTeam(Team team)
        {
            SelectedTeam = team;
            IsTeamDropdownOpen = false;
        }

        [RelayCommand]
        private async Task EditTeam(Team team)
        {
            RequestClosePopup?.Invoke();
            await Task.Delay(200);

            var member = team.Members?.FirstOrDefault();
            if (member != null)
            {
                var editPopup = new Views.Popups.ModifyTeamPopup(team, member, this);
                Shell.Current.CurrentPage.ShowPopup(editPopup);
            }
        }

        [RelayCommand]
        private async Task RemoveMember(Team team)
        {
            RequestClosePopup?.Invoke();
            await Task.Delay(200);

            var member = team.Members?.FirstOrDefault();

            if (member != null)
            {
                var removePopup = new Views.Popups.RemoveMemberPopup(team, member, this);
                Shell.Current.CurrentPage.ShowPopup(removePopup);
            }
        }   
    }
}