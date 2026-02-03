
    using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.DTOs.Requests;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using System.Collections.ObjectModel;

namespace kando_desktop.ViewModels.Popups
{
    public partial class CreateTeamPopupViewModel : ObservableObject
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly ITeamService _teamService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        private string teamName;

        [ObservableProperty]
        private string selectedIconSource = "cat.png";

        [ObservableProperty]
        private Color selectedTeamColor = Color.FromArgb("#8f45ef");

        [ObservableProperty]
        private bool hasNameError;

        [ObservableProperty] 
        private bool isBusy;

        public Action RequestClose;

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

        public ObservableCollection<ColorItem> Colors { get; } = new()
        {
            new ColorItem { ColorHex = "#8f45ef", IsSelected = true },
            new ColorItem { ColorHex = "#e7336a" },
            new ColorItem { ColorHex = "#158ddf" },
            new ColorItem { ColorHex = "#ee770b" },
            new ColorItem { ColorHex = "#13ad64" },
        };

        public CreateTeamPopupViewModel(
            IWorkspaceService workspaceService,
            INotificationService notificationService,
            ITeamService teamService, 
            ISessionService sessionService)
        {
            _workspaceService = workspaceService;
            _notificationService = notificationService;
            _teamService = teamService;
            _sessionService = sessionService;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Create()
        {
            if (string.IsNullOrWhiteSpace(TeamName))
            {
                HasNameError = true;
                return;
            }

            IsBusy = true;
            HasNameError = false;

            try
            {
                var team = new CreateTeamDto
                {
                    Name = TeamName,
                    Icon = SelectedIconSource,
                    Color = SelectedTeamColor.ToHex()
                };

                var createdTeamResponse = await _teamService.CreateTeamAsync(team);

                if (createdTeamResponse != null)
                {
                    var currentUser = _sessionService.CurrentUser;

                    _workspaceService.CreateTeam(createdTeamResponse, currentUser);
                    RequestClose?.Invoke();
                    _notificationService.Show(AppResources.TeamCreatedSuccessfully);
                }
                else
                {
                    _notificationService.Show(AppResources.FailedToCreateTeam, true);
                }

            } catch (Exception ex) {
                _notificationService.Show(AppResources.UnexpectedError);
            } finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke();
        }

        [RelayCommand]
        private void SelectIcon(IconItem icon)
        {
            foreach (var item in Icons)
            {
                item.IsSelected = false;
            }

            icon.IsSelected = true;
            SelectedIconSource = icon.Source;
        }

        [RelayCommand]
        private void SelectColor(ColorItem colorItem)
        {
            foreach (var item in Colors)
            {
                item.IsSelected = false;
            }

            colorItem.IsSelected = true;
            SelectedTeamColor = colorItem.Color;
        }

        partial void OnTeamNameChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                HasNameError = false;
            }
        }
    }
}