using kando_desktop.Services.Contracts;

namespace kando_desktop
{
    public partial class App : Application
    {
        private readonly ISessionService _sessionService;

        private readonly INotificationService _notificationService;

        public App(ISessionService sessionService, INotificationService notificationService)
        {
            InitializeComponent();
            _sessionService = sessionService;
            _notificationService = notificationService;

            Task.Run(async () => await InitializeSession());
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
                    var token = await SecureStorage.GetAsync("auth_token");
                    var userId = _sessionService.CurrentUser?.UserId ?? 0;

                    _ = _notificationService.LoadHistoricalNotificationsAsync();
                    _ = _notificationService.InitializeSignalRAsync(token, userId);

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.GoToAsync("//HomePage", false);
                    });
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.GoToAsync("//LoginPage", false);
                    });
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () => await Shell.Current.GoToAsync("//LoginPage"));
            }
        }

        private async Task InitializeNotifications()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                var userIdStr = Preferences.Get("user_id", 0);

                if (!string.IsNullOrEmpty(token) && userIdStr > 0)
                {
                    await _notificationService.LoadHistoricalNotificationsAsync();

                    await _notificationService.InitializeSignalRAsync(token, userIdStr);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}