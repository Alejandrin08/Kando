using kando_desktop.Services.Contracts;
using kando_desktop.ViewModels.ContentPages;

namespace kando_desktop.Views.ContentPages
{
    public partial class ModifyProfilePage : ContentPage
    {
        private readonly INotificationService _notificationService;

        public ModifyProfilePage(ModifyProfileViewModel modifyBoardPopupViewModel, INotificationService notificationService)
        {
            InitializeComponent();
            BindingContext = modifyBoardPopupViewModel;
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