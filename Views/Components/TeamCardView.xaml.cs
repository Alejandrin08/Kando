using CommunityToolkit.Maui.Views;
using kando_desktop.Helpers;
using kando_desktop.Models;
using kando_desktop.ViewModels;
using kando_desktop.Views.Popups;

namespace kando_desktop.Views.Components;

public partial class TeamCardView : ContentView
{
	public TeamCardView()
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

        var team = this.BindingContext as Team;

        var homeViewModel = ServiceHelper.GetService<HomeViewModel>();

        if (team != null && homeViewModel != null)
        {
            var popup = new TeamMenuPopup(team, homeViewModel);

            popup.Anchor = anchor;

            Page currentPage = Shell.Current.CurrentPage ?? App.Current.MainPage;
            currentPage.ShowPopup(popup);
        }
    }
}