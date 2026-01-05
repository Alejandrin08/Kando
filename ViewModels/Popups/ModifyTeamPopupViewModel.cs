using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.Resources.Strings;
using System.Collections.ObjectModel;
using System.Linq;

namespace kando_desktop.ViewModels.Popups
{
    public partial class ModifyTeamPopupViewModel : ObservableObject
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly Team _teamToEdit;

        [ObservableProperty]
        private string teamName;

        [ObservableProperty]
        private string selectedIconSource;

        [ObservableProperty]
        private Color selectedTeamColor;

        [ObservableProperty]
        private bool hasNameError;

        public Action RequestClose;

        public ObservableCollection<IconItem> Icons { get; } = new()
        {
            new IconItem { Source = "cat.png" },
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
            new ColorItem { ColorHex = "#8f45ef" },
            new ColorItem { ColorHex = "#e7336a" },
            new ColorItem { ColorHex = "#158ddf" },
            new ColorItem { ColorHex = "#ee770b" },
            new ColorItem { ColorHex = "#13ad64" },
        };

        public ModifyTeamPopupViewModel(
            Team team,
            IWorkspaceService workspaceService,
            INotificationService notificationService)
        {
            _teamToEdit = team;
            _workspaceService = workspaceService;
            _notificationService = notificationService;

            TeamName = team.Name;
            SelectedIconSource = !string.IsNullOrEmpty(team.Icon) ? team.Icon : "cat.png";
            SelectedTeamColor = team.TeamColor;

            var iconToSelect = Icons.FirstOrDefault(i => i.Source == SelectedIconSource) ?? Icons.First();
            iconToSelect.IsSelected = true;

            var colorToSelect = Colors.FirstOrDefault(c => AreColorsEqual(c.Color, SelectedTeamColor)) ?? Colors.First();
            colorToSelect.IsSelected = true;
        }

        private bool AreColorsEqual(Color a, Color b)
        {
            return Math.Abs(a.Red - b.Red) < 0.01 &&
                   Math.Abs(a.Green - b.Green) < 0.01 &&
                   Math.Abs(a.Blue - b.Blue) < 0.01;
        }

        [RelayCommand]
        private void Modify()
        {
            if (string.IsNullOrWhiteSpace(TeamName))
            {
                HasNameError = true;
                return;
            }

            HasNameError = false;

            _workspaceService.UpdateTeam(_teamToEdit, TeamName, SelectedIconSource, SelectedTeamColor);

            var message = AppResources.TeamUpdatedSuccessfully;
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
        private void SelectColor(ColorItem colorItem)
        {
            foreach (var item in Colors) item.IsSelected = false;
            colorItem.IsSelected = true;
            SelectedTeamColor = colorItem.Color;
        }

        partial void OnTeamNameChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) HasNameError = false;
        }
    }
}