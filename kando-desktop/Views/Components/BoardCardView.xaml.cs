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
}