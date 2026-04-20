using kando_desktop.Services.Contracts;
using kando_desktop.Services.Implementations;
using kando_desktop.ViewModels.ContentPages;

namespace kando_desktop.Views.ContentPages
{
    public partial class ResetPasswordPage : ContentPage
    {

        private readonly INotificationService _notificationService;

        public ResetPasswordPage(ResetPasswordViewModel resetPasswordViewModel, INotificationService notificationService)
        {
            InitializeComponent();
            BindingContext = resetPasswordViewModel;
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