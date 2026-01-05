using kando_desktop.ViewModels;
using CommunityToolkit.Maui.Views;
using kando_desktop.Views.Popups;
using kando_desktop.Services.Contracts;

namespace kando_desktop.Views;

public partial class HomePage : ContentPage
{

	private HomeViewModel _viewModel;

    private readonly INotificationService _notificationService;

    public HomePage(HomeViewModel homeViewModel, INotificationService notificationService)
	{
		InitializeComponent();
		_viewModel = homeViewModel;
        BindingContext = homeViewModel;
        _notificationService = notificationService;

        _notificationService.OnShowNotification += async (msg, isError) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await NotificationToast.ShowToast(msg, isError);
            });
        };

        _viewModel.RequestMenuOpen += OnRequestMenuOpen;
        _viewModel.RequestShowCreateTeam += OnRequestShowCreateTeam;
        _viewModel.RequestShowCreateBoard += OnRequestShowCreateBoard;
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

    private void OnRequestShowCreateTeam()
    {
        var popup = new CreateTeamPopup(_viewModel);
        this.ShowPopup(popup);
    }

    private void OnRequestShowCreateBoard()
    {
        var popup = new CreateBoadPopup(_viewModel);
        this.ShowPopup(popup);
    }

    protected override void OnDisappearing()
    {
        _viewModel.RequestMenuOpen -= OnRequestMenuOpen;
        _viewModel.RequestShowCreateTeam -= OnRequestShowCreateTeam;
        _viewModel.RequestShowCreateBoard -= OnRequestShowCreateBoard;
        base.OnDisappearing();
    }
}