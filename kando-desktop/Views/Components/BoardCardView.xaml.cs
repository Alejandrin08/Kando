using CommunityToolkit.Maui.Views;
using kando_desktop.Models;
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

            Page currentPage = Shell.Current.CurrentPage ?? App.Current.MainPage;
            currentPage.ShowPopup(popup);
        }
    }
}