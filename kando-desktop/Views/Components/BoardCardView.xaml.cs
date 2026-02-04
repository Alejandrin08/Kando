using CommunityToolkit.Maui.Views;
using kando_desktop.Helpers;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.ViewModels.Popups;
using kando_desktop.Views.Popups;

namespace kando_desktop.Views.Components;

public partial class BoardCardView : ContentView
{
	public BoardCardView()
	{
		InitializeComponent();
	}
    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        ThreeDotsLabel.Opacity = 1;
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        ThreeDotsLabel.Opacity = 0;
    }

    private void OnOptionsTapped(object sender, EventArgs e)
    {
        var anchor = sender as View;
        var board = this.BindingContext as Board;

        if (board != null)
        {
            var viewModel = new BoardMenuPopupViewModel(board);
            var popup = new BoardMenuPopup();
            popup.BindingContext = viewModel;
            popup.Anchor = anchor;

            viewModel.RequestClose = () => popup.Close();

            viewModel.RequestEditBoard = (selectedBoard) =>
            {
                ShowModifyBoardPopup(selectedBoard);
            };

            viewModel.RequestDeleteBoard = (selectedBoard) =>
            {
                ShowDeleteBoardPopup(selectedBoard);
            };

            Page currentPage = Shell.Current.CurrentPage ?? App.Current.MainPage;
            currentPage.ShowPopup(popup);
        }
    }

    private void ShowModifyBoardPopup(Board board)
    {
        var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
        var notificationService = ServiceHelper.GetService<INotificationService>();
        var boardService = ServiceHelper.GetService<IBoardService>();

        if (workspaceService != null && notificationService != null)
        {
            var viewModel = new ModifyBoardPopupViewModel(
                board,
                workspaceService,
                notificationService,
                boardService);

            var popup = new ModifyBoardPopup();
            popup.BindingContext = viewModel;

            viewModel.RequestClose = () => popup.Close();

            Page currentPage = Shell.Current.CurrentPage ?? App.Current.MainPage;
            currentPage.ShowPopup(popup);
        }
    }


    private void ShowDeleteBoardPopup(Board board)
    {
        var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
        var notificationService = ServiceHelper.GetService<INotificationService>();
        var boardService = ServiceHelper.GetService<IBoardService>();

        if (workspaceService != null && notificationService != null)
        {
            var viewModel = new DeleteBoardPopupViewModel(
                workspaceService,
                notificationService,
                boardService,
                board);

            var popup = new DeleteBoardPopup();
            popup.BindingContext = viewModel;

            viewModel.RequestClose = () => popup.Close();

            Page currentPage = Shell.Current.CurrentPage ?? App.Current.MainPage;
            currentPage.ShowPopup(popup);
        }
    }
}