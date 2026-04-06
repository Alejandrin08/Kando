using kando_desktop.Helpers;
using kando_desktop.Services.Contracts;

#if WINDOWS
using Microsoft.UI.Windowing;
using WinUIWindow = Microsoft.UI.Xaml.Window;
#endif

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

#if WINDOWS
            window.HandlerChanged += (s, e) =>
            {
                var nativeWindow = window.Handler?.PlatformView as WinUIWindow;
                if (nativeWindow == null) return;

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                if (appWindow?.Presenter is OverlappedPresenter presenter)
                {
                    presenter.SetBorderAndTitleBar(true, true);
                }
            };
#endif

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

                    await _notificationService.InitializeSignalRAsync(token, userId);
                    _ = _notificationService.LoadHistoricalNotificationsAsync();

                    var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
                    await workspaceService.InitializeDataAsync();

                    var teamIds = workspaceService.Teams.Select(t => t.Id).ToList();
                    await _notificationService.SubscribeToTeamsAsync(teamIds);

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
                MainThread.BeginInvokeOnMainThread(async () =>
                    await Shell.Current.GoToAsync("//LoginPage"));
            }
        }
    }
}