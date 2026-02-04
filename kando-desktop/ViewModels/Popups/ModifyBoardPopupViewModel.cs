using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.DTOs.Requests;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using System.Collections.ObjectModel;

namespace kando_desktop.ViewModels.Popups
{
    public partial class ModifyBoardPopupViewModel : ObservableObject
    {

        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly IBoardService _boardService;
        private readonly Board _boardToEdit;

        [ObservableProperty]
        private string boardName;

        [ObservableProperty]
        private string selectedIconSource;

        [ObservableProperty]
        private bool hasNameError;

        [ObservableProperty]
        private bool isBusy;

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

        public ModifyBoardPopupViewModel(
            Board board,
            IWorkspaceService workspaceService,
            INotificationService notificationService,
            IBoardService boardService)
        {
            _boardToEdit = board;
            _workspaceService = workspaceService;
            _notificationService = notificationService;

            BoardName = board.Name;
            SelectedIconSource = !string.IsNullOrEmpty(board.Icon) ? board.Icon : "cat.png";

            var iconToSelect = Icons.FirstOrDefault(i => i.Source == SelectedIconSource) ?? Icons.First();
            iconToSelect.IsSelected = true;

            _boardService = boardService;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Modify()
        {
            if (string.IsNullOrWhiteSpace(BoardName))
            {
                HasNameError = true;
                return;
            }

            IsBusy = true;
            HasNameError = false;

            var boardToUpdate = new UpdateBoardDto()
            {
                Name = BoardName,
                Icon = SelectedIconSource,
            };

            try
            {

                var success = await _boardService.UpdateBoardAsync(_boardToEdit.Id, boardToUpdate);

                if (success)
                {
                    _workspaceService.UpdateBoard(_boardToEdit.Id, boardToUpdate);
                    RequestClose?.Invoke();
                    _notificationService.Show(AppResources.BoardUpdatedSuccessfully);
                }
                else
                {
                    _notificationService.Show(AppResources.FailedToUpdateBoard, true);
                }
            }
            catch (Exception ex)
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

        partial void OnBoardNameChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) HasNameError = false;
        }
    }
}
