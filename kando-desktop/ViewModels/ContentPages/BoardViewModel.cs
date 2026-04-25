using CommunityToolkit.Mvvm.ComponentModel;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.ContentPages
{
    [QueryProperty(nameof(BoardId), "SelectedBoardId")]
    [QueryProperty(nameof(TeamId), "BoardTeamId")]
    public partial class BoardViewModel : BaseViewModel
    {

        private readonly INotificationService _notificationService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        private int boardId;

        [ObservableProperty]
        private int teamId;

        public BoardViewModel(INotificationService notificationService, ISessionService sessionService) : base(sessionService)
        {
            _notificationService = notificationService;
            _sessionService = sessionService;
        }
    }
}