using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.ContentPages
{
    public partial class ModifyProfileViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private readonly ISessionService _sessionService;

        public ModifyProfileViewModel(INotificationService notificationService, ISessionService sessionService) : base(sessionService)
        {
            _notificationService = notificationService;
            _sessionService = sessionService;
        }
    }
}