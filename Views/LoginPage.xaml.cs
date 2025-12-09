using kando_desktop.ViewModels;
using kando_desktop.Controls;

namespace kando_desktop.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel loginViewModel)
	{
		InitializeComponent();
		BindingContext = loginViewModel;

        UpdateDrawableTheme(Application.Current.RequestedTheme);
        Application.Current.RequestedThemeChanged += OnThemeChanged;
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
}