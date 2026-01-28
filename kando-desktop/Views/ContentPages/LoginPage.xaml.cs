using kando_desktop.ViewModels;
using kando_desktop.Controls;
using kando_desktop.Services.Contracts;
using kando_desktop.ViewModels.ContentPages;

namespace kando_desktop.Views.ContentPages;

public partial class LoginPage : ContentPage
{
    private readonly INotificationService _notificationService;
    public LoginPage(LoginViewModel loginViewModel, INotificationService notificationService)
	{
		InitializeComponent();
		BindingContext = loginViewModel;
        _notificationService = notificationService;

        UpdateDrawableTheme(Application.Current.RequestedTheme);
        Application.Current.RequestedThemeChanged += OnThemeChanged;

        _notificationService.OnShowNotification += async (msg, isError) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await NotificationToast.ShowToast(msg, isError);
            });
        };
    }

    private void OnThemeChanged(object sender, AppThemeChangedEventArgs e)
    {
        UpdateDrawableTheme(e.RequestedTheme);
    }

    private void UpdateDrawableTheme(AppTheme theme)
    {
        if (BgGraphicsView.Drawable is GridBackgroundDrawable drawable)
        {
            drawable.IsDarkTheme = (theme == AppTheme.Dark);

            BgGraphicsView.Invalidate();
        }
    }

    protected override void OnDisappearing()
    {
        Application.Current.RequestedThemeChanged -= OnThemeChanged;
        base.OnDisappearing();
    }
}