using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Helpers;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.Components
{
    public partial class SidebarViewModel : ObservableObject
    {

        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        private ObservableCollection<Board> teamBoards = new();

        [ObservableProperty]
        private Board selectedBoard;

        public Action RequestClose;

        public SidebarViewModel()
        {
            _workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            _notificationService = ServiceHelper.GetService<INotificationService>();
            _sessionService = ServiceHelper.GetService<ISessionService>();
        }

        [RelayCommand]
        private async Task NavigateToHome()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private void SelectBoard(Board board)
        {
            if (board == null) return;

            foreach (var b in TeamBoards)
                b.IsSelected = false;

            board.IsSelected = true;
            SelectedBoard = board;
        }

        partial void OnSelectedBoardChanged(Board value)
        {
            if (value != null)
            {
                foreach (var b in TeamBoards)
                    b.IsSelected = b.Id == value.Id;

                Console.WriteLine($"Tablero cambiado desde el sidebar. Nuevo ID: {value.Id}");
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            RequestClose?.Invoke();

            await Task.Delay(150);

            await _notificationService.DisconnectSignalRAsync();
            _notificationService.ClearNotifications();
            _workspaceService.ClearData();
            _sessionService.Logout();
            await Shell.Current.GoToAsync("//LoginPage");

            var message = AppResources.Logout;
            _notificationService.Show(message);
        }

        public void LoadBoards(int teamId, int currentBoardId)
        {
            TeamBoards = _workspaceService.GetBoardsTeam(teamId);
            SelectedBoard = TeamBoards.FirstOrDefault(b => b.Id == currentBoardId);
        }
    }
}