using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace kando_desktop.ViewModels.ContentPages
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;

        public ObservableCollection<Board> FilteredBoards { get; } = new();
        public ObservableCollection<Team> Teams => _workspaceService.Teams;

        public Action RequestShowCreateTeam;
        public Action RequestShowCreateBoard;

        [ObservableProperty] private bool isTeamDropdownOpen;
        [ObservableProperty] private Team selectedTeam;
        [ObservableProperty] private string searchText;

        [ObservableProperty] private int activeBoardsCount;
        [ObservableProperty] private int completedTasks;
        [ObservableProperty] private int totalTasksCount;
        [ObservableProperty] private int teamsCount;

        [ObservableProperty]
        private bool isBusy;

        public HomeViewModel(IWorkspaceService workspaceService, INotificationService notificationService, ISessionService sessionService) : base(sessionService)
        {
            _workspaceService = workspaceService;
            _notificationService = notificationService;

            _workspaceService.PropertyChanged += OnServicePropertyChanged;

            _workspaceService.Boards.CollectionChanged += OnDataChanged;
            _workspaceService.Teams.CollectionChanged += OnDataChanged;

            LoadData();
        }

        private async void LoadData()
        {
            IsBusy = true;
            await _workspaceService.InitializeDataAsync();
            RefreshData(); 
            IsBusy = false;
        }

        private void OnDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsBusy) return;

            RefreshData();
        }

        private void OnServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IWorkspaceService.Boards) ||
                e.PropertyName == nameof(IWorkspaceService.Teams))
            {
                if (e.PropertyName == nameof(IWorkspaceService.Boards))
                {
                    _workspaceService.Boards.CollectionChanged -= OnDataChanged;
                    _workspaceService.Boards.CollectionChanged += OnDataChanged;
                }

                if (e.PropertyName == nameof(IWorkspaceService.Teams))
                {
                    _workspaceService.Teams.CollectionChanged -= OnDataChanged;
                    _workspaceService.Teams.CollectionChanged += OnDataChanged;
                }

                OnPropertyChanged(nameof(Teams));
                OnPropertyChanged(nameof(FilteredBoards)); 

                RefreshData();
            }
        }

        public void Unsubscribe()
        {
            _workspaceService.PropertyChanged -= OnServicePropertyChanged;
            _workspaceService.Boards.CollectionChanged -= OnDataChanged;
            _workspaceService.Teams.CollectionChanged -= OnDataChanged;
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

            TotalTasksCount = _workspaceService.Boards.Sum(b => b.TotalTasks);
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
    }
}