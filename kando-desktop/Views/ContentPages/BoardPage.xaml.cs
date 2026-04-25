using kando_desktop.Services.Contracts;
using kando_desktop.ViewModels.ContentPages;

namespace kando_desktop.Views.ContentPages
{
    public partial class BoardPage : ContentPage
    {
        private readonly INotificationService _notificationService;

        private bool _isSidebarExpanded = true;

        public BoardPage(INotificationService notificationService, BoardViewModel boardViewModel)
        {
            InitializeComponent();
            BindingContext = boardViewModel;
            _notificationService = notificationService;

            _notificationService.OnShowNotification += async (msg, isError) =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await NotificationToast.ShowToast(msg, isError);
                });
            };
        }

        private void OnToggleSidebarTapped(object sender, TappedEventArgs e)
        {
            _isSidebarExpanded = !_isSidebarExpanded;

            SidebarComponent.IsExpanded = _isSidebarExpanded;

            uint duration = 750;

            if (_isSidebarExpanded)
            {
                ToggleIcon.RotateTo(90, duration, Easing.CubicOut);

                var animation = new Animation(v => SidebarComponent.WidthRequest = v, 70, 280);
                animation.Commit(this, "SidebarAnim", 16, duration, Easing.CubicOut);
            }
            else
            {
                ToggleIcon.RotateTo(270, duration, Easing.CubicOut);

                var animation = new Animation(v => SidebarComponent.WidthRequest = v, 280, 70);
                animation.Commit(this, "SidebarAnim", 16, duration, Easing.CubicOut);
            }
        }
    }
}