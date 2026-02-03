using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.Resources.Strings;
using System.Collections.ObjectModel;
using kando_desktop.DTOs.Requests;

namespace kando_desktop.ViewModels.Popups
{
    public partial class CreateBoardPopupViewModel : ObservableObject
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly IBoardService _boardService;

        [ObservableProperty]
        private string boardName;

        [ObservableProperty]
        private string selectedIconSource = "cat.png";

        [ObservableProperty]
        private Team selectedTeam;

        [ObservableProperty]
        private bool hasNameError;

        [ObservableProperty]
        private bool isBusy;

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
            IBoardService boardService,
            Team selectedTeam)
        {
            _workspaceService = workspaceService;
            _notificationService = notificationService;
            _boardService = boardService;
            SelectedTeam = selectedTeam;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Create()
        {
            if (string.IsNullOrWhiteSpace(BoardName))
            {
                HasNameError = true;
                return;
            }

            if (SelectedTeam == null)
            {
                _notificationService.Show(AppResources.SelectTeamFirst, true);
                return;
            }

            HasNameError = false;
            IsBusy = true;

            try
            {
                var dto = new CreateBoardDto
                {
                    Name = BoardName,
                    Icon = SelectedIconSource,
                    TeamId = SelectedTeam.Id 
                };

                var createdBoard = await _boardService.CreateBoardAsync(dto);

                if (createdBoard != null)
                {
                    _workspaceService.CreateBoard(createdBoard);
                    RequestClose?.Invoke();
                    _notificationService.Show(AppResources.BoardCreatedSuccessfully);
                }
                else
                {
                    _notificationService.Show(AppResources.FailedToCreateBoard, true);
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