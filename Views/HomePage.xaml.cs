using kando_desktop.ViewModels;
using CommunityToolkit.Maui.Views;
using kando_desktop.Views.Popups;

namespace kando_desktop.Views;

public partial class HomePage : ContentPage
{

	private HomeViewModel _viewModel;
    public HomePage(HomeViewModel homeViewModel)
	{
		InitializeComponent();
		_viewModel = homeViewModel;
        BindingContext = homeViewModel;

        _viewModel.RequestMenuOpen += OnRequestMenuOpen;
    }
    private void OnRequestMenuOpen(object anchor)
    {
        var popup = new ProfileMenuPopup(_viewModel);

        if (anchor is View anchorView)
        {
            popup.Anchor = anchorView;
            popup.HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.End;
            popup.VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.End;

        }

        this.ShowPopup(popup);
    }

    protected override void OnDisappearing()
    {
        _viewModel.RequestMenuOpen -= OnRequestMenuOpen;
        base.OnDisappearing();
    }
}