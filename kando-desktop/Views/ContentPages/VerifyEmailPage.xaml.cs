using kando_desktop.Services.Contracts;
using kando_desktop.Services.Implementations;
using kando_desktop.ViewModels.ContentPages;

namespace kando_desktop.Views.ContentPages
{
    public partial class VerifyEmailPage : ContentPage
    {

        private readonly INotificationService _notificationService;

        public VerifyEmailPage(VerifyEmailViewModel verifyEmailViewModel, INotificationService notificationService)
        {
            InitializeComponent();
            BindingContext = verifyEmailViewModel;
            _notificationService = notificationService;
            _notificationService.OnShowNotification += async (msg, isError) =>
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await NotificationToast.ShowToast(msg, isError);
                    });
                };
        }
    }
}