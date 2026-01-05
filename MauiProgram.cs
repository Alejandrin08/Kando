using kando_desktop.Controls;
using kando_desktop.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using kando_desktop.Services.Contracts;
using kando_desktop.Services.Implementations;
using kando_desktop.ViewModels.ContentPages;
using kando_desktop.ViewModels.Popups;
using kando_desktop.Views.ContentPages;

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

            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            builder.Services.AddTransient<CreateTeamPopupViewModel>();
            builder.Services.AddTransient<CreateBoardPopupViewModel>();
            builder.Services.AddTransient<ModifyTeamPopupViewModel>();
            builder.Services.AddTransient<ProfileMenuPopupViewModel>();
            builder.Services.AddTransient<RemoveMemberPopupViewModel>();
            builder.Services.AddTransient<TeamMenuPopupViewModel>();

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            builder.Services.AddSingleton<IWorkspaceService, WorkspaceService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();

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
