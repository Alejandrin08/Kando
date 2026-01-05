using kando_desktop.Controls;
using kando_desktop.ViewModels.ContentPages;

namespace kando_desktop.Views.ContentPages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel registerViewModel)
	{
        InitializeComponent();
        BindingContext = registerViewModel;

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