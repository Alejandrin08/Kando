using kando_desktop.Services.Contracts;

namespace kando_desktop
{
    public partial class App : Application
    {
        private readonly ISessionService _sessionService;

        public App(ISessionService sessionService)
        {
            InitializeComponent();
            _sessionService = sessionService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            window.Width = 1080;
            window.Height = 720;
            window.MinimumWidth = 800;
            window.MinimumHeight = 600;

            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            var screenWidth = displayInfo.Width / displayInfo.Density;
            var screenHeight = displayInfo.Height / displayInfo.Density;
            window.X = (screenWidth - window.Width) / 2;
            window.Y = (screenHeight - window.Height) / 2;

            return window;
        }

        protected override async void OnStart()
        {
            base.OnStart();

            await Task.Delay(100);

            await InitializeSession();
        }

        private async Task InitializeSession()
        {
            try
            {
                await _sessionService.LoadSessionAsync();

                if (_sessionService.IsAuthenticated)
                {
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inicializando sesión: {ex.Message}");
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}