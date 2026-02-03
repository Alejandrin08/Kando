using CommunityToolkit.Maui;
using kando_desktop.Controls;
using kando_desktop.Services.Contracts;
using kando_desktop.Services.Implementations;
using kando_desktop.ViewModels.ContentPages;
using kando_desktop.ViewModels.Popups;
using kando_desktop.Views.ContentPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace kando_desktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("kando_desktop.appsettings.json");

            if (stream != null)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
                builder.Configuration.AddConfiguration(config);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: No se encontró appsettings.json.");
            }

            string baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

            builder.Services.AddTransient<AuthenticatedHttpMessageHandler>();

            Action<HttpClient> httpClientConfig = client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            };

            Func<HttpClientHandler> handlerConfig = () => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            builder.Services.AddHttpClient<IAuthService, AuthService>(httpClientConfig)
                   .ConfigurePrimaryHttpMessageHandler(handlerConfig);

            builder.Services.AddHttpClient<ITeamService, TeamService>(httpClientConfig)
                   .ConfigurePrimaryHttpMessageHandler(handlerConfig)
                   .AddHttpMessageHandler<AuthenticatedHttpMessageHandler>();

            builder.Services.AddHttpClient<IUserService, UserService>(httpClientConfig)
                   .ConfigurePrimaryHttpMessageHandler(handlerConfig)
                   .AddHttpMessageHandler<AuthenticatedHttpMessageHandler>();

            builder.Services.AddHttpClient<IBoardService, BoardService>(httpClientConfig)
                    .ConfigurePrimaryHttpMessageHandler(handlerConfig)
                    .AddHttpMessageHandler<AuthenticatedHttpMessageHandler>();

            builder.Services.AddSingleton<ISessionService, SessionService>();
            builder.Services.AddSingleton<IWorkspaceService, WorkspaceService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();

            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<HomeViewModel>();

            builder.Services.AddTransient<CreateTeamPopupViewModel>();
            builder.Services.AddTransient<CreateBoardPopupViewModel>();
            builder.Services.AddTransient<ModifyTeamPopupViewModel>();
            builder.Services.AddTransient<ProfileMenuPopupViewModel>();
            builder.Services.AddTransient<RemoveMemberPopupViewModel>();
            builder.Services.AddTransient<TeamMenuPopupViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<HomePage>();

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
                if (view is BorderlessEntry)
                {
#if WINDOWS
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.Style = null;
#endif
#if ANDROID
                    handler.PlatformView.Background = null;
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
#if IOS || MACCATALYST
                    handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
                }
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}