using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using kando_desktop.Helpers;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.ContentPages
{
    [QueryProperty(nameof(BoardId), "SelectedBoardId")]
    [QueryProperty(nameof(TeamId), "BoardTeamId")]
    [QueryProperty(nameof(BoardName), "SelectedBoardName")]
    public partial class BoardViewModel : BaseViewModel
    {

        private readonly INotificationService _notificationService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        private int boardId;

        [ObservableProperty]
        private int teamId;

        [ObservableProperty]
        private string boardName;

        public BoardViewModel(INotificationService notificationService, ISessionService sessionService) : base(sessionService)
        {
            _notificationService = notificationService;
            _sessionService = sessionService;

            WeakReferenceMessenger.Default.Register<BoardChangedMessage>(this, (recipient, message) =>
            {
                var newBoard = message.Value;

                BoardId = newBoard.Id;
                BoardName = newBoard.Name;
            });
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private void CreateColumn()
        {

        }
    }
}