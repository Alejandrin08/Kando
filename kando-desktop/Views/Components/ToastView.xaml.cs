namespace kando_desktop.Views.Components;

public partial class ToastView : ContentView
{
    private CancellationTokenSource _cancellationTokenSource;

    public ToastView()
    {
        InitializeComponent();
    }

    public async Task ShowToast(string message, bool isError)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        MessageLabel.Text = message;
        bool isDarkTheme = Application.Current.RequestedTheme == AppTheme.Dark;

        string colorSuffix = isDarkTheme ? "_white.png" : "_black.png";

        string iconBase = isError ? "error" : "check";

        IconImage.Source = $"{iconBase}{colorSuffix}";

        Container.Opacity = 0;
        Container.TranslationY = 100; 

        await Task.WhenAll(
            Container.FadeTo(1, 250, Easing.CubicOut),
            Container.TranslateTo(0, 0, 250, Easing.CubicOut)
        );

        try
        {
            await Task.Delay(4000, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (!token.IsCancellationRequested)
        {
            await Task.WhenAll(
                Container.FadeTo(0, 250, Easing.CubicIn),
                Container.TranslateTo(0, 100, 250, Easing.CubicIn)
            );
        }
    }
}