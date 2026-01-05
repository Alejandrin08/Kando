using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.Resources.Strings;
using System.Collections.ObjectModel;

namespace kando_desktop.ViewModels.Popups
{
    public partial class CreateBoardPopupViewModel : ObservableObject
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string boardName;

        [ObservableProperty]
        private string selectedIconSource = "cat.png";

        [ObservableProperty]
        private Team selectedTeam;

        [ObservableProperty]
        private bool hasNameError;

        [ObservableProperty] 
        private bool isTeamDropdownOpen;

        public Action RequestClose;

        public ObservableCollection<Team> Teams => _workspaceService.Teams;

        public ObservableCollection<IconItem> Icons { get; } = new()
        {
            new IconItem { Source = "cat.png", IsSelected = true },
            new IconItem { Source = "tulip.png" },
            new IconItem { Source = "dog.png" },
            new IconItem { Source = "elephant.png" },
            new IconItem { Source = "hacker.png" },
            new IconItem { Source = "ninja.png" },
            new IconItem { Source = "penguin.png" },
            new IconItem { Source = "linux.png" },
            new IconItem { Source = "shark.png" },
            new IconItem { Source = "startup.png" },
            new IconItem { Source = "puzzle.png" },
            new IconItem { Source = "puzzle_two.png" },
            new IconItem { Source = "swords.png" },
            new IconItem { Source = "alien.png" },
        };

        public CreateBoardPopupViewModel(
            IWorkspaceService workspaceService,
            INotificationService notificationService,
            Team selectedTeam)
        {
            _workspaceService = workspaceService;
            _notificationService = notificationService;
            SelectedTeam = selectedTeam;
        }

        [RelayCommand]
        private void Create()
        {
            if (string.IsNullOrWhiteSpace(BoardName))
            {
                HasNameError = true;
                return;
            }

            if (SelectedTeam == null)
            {
                var messageError = AppResources.SelectTeamFirst;
                _notificationService.Show(messageError, true);
                return;
            }

            HasNameError = false;

            _workspaceService.CreateBoard(BoardName, SelectedIconSource, SelectedTeam);

            var message = AppResources.BoardCreatedSuccessfully;
            _notificationService.Show(message);

            RequestClose?.Invoke();
        }

        [RelayCommand]
        private void Close() => RequestClose?.Invoke();

        [RelayCommand]
        private void SelectIcon(IconItem icon)
        {
            foreach (var item in Icons) item.IsSelected = false;
            icon.IsSelected = true;
            SelectedIconSource = icon.Source;
        }

        [RelayCommand]
        private void ToggleTeamDropdown() => IsTeamDropdownOpen = !IsTeamDropdownOpen;

        [RelayCommand]
        private void SelectTeam(Team team)
        {
            SelectedTeam = team;
            IsTeamDropdownOpen = false;
        }

        partial void OnBoardNameChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) HasNameError = false;
        }
    }
}