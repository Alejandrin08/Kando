using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Helpers;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.ViewModels.Popups;
using kando_desktop.Views.Popups;

namespace kando_desktop.ViewModels.Components
{
    public partial class BoardCardViewModel : ObservableObject
    {
        [ObservableProperty]
        private Board _board;

        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly IBoardService _boardService;

        public BoardCardViewModel(Board board)
        {
            _board = board;
            _workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            _notificationService = ServiceHelper.GetService<INotificationService>();
            _boardService = ServiceHelper.GetService<IBoardService>();
        }

        [RelayCommand]
        private void OpenOptions(View anchor)
        {
            var viewModel = new BoardMenuPopupViewModel(Board);
            var popup = new BoardMenuPopup
            {
                BindingContext = viewModel,
                Anchor = anchor
            };

            viewModel.RequestClose = () => popup.Close();

            viewModel.RequestEditBoard = (b) => ShowModifyBoardPopup();
            viewModel.RequestDeleteBoard = (b) => ShowDeleteBoardPopup();

            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        private void ShowModifyBoardPopup()
        {
            var viewModel = new ModifyBoardPopupViewModel(Board, _workspaceService, _notificationService, _boardService);
            var popup = new ModifyBoardPopup { BindingContext = viewModel };
            viewModel.RequestClose = () => popup.Close();
            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        private void ShowDeleteBoardPopup()
        {
            var viewModel = new DeleteBoardPopupViewModel(_workspaceService, _notificationService, _boardService, Board);
            var popup = new DeleteBoardPopup { BindingContext = viewModel };
            viewModel.RequestClose = () => popup.Close();
            Shell.Current.CurrentPage.ShowPopup(popup);
        }
    }
}