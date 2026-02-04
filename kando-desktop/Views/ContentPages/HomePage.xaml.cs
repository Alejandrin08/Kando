using kando_desktop.ViewModels.ContentPages;
using kando_desktop.ViewModels.Popups;
using CommunityToolkit.Maui.Views;
using kando_desktop.Views.Popups;
using kando_desktop.Services.Contracts;
using kando_desktop.Helpers;

namespace kando_desktop.Views.ContentPages
{
    public partial class HomePage : ContentPage
    {
        private HomeViewModel _viewModel;
        private readonly INotificationService _notificationService;
        private readonly IWorkspaceService _workspaceService;
        private readonly ITeamService _teamService;
        private readonly IBoardService _boardService;
        private readonly ISessionService _sessionService;

        public HomePage(
            HomeViewModel homeViewModel,
            INotificationService notificationService,
            IWorkspaceService workspaceService,
            ITeamService teamService,
            IBoardService boardService,
            ISessionService sessionService)
        {
            InitializeComponent();
            _viewModel = homeViewModel;
            _notificationService = notificationService;
            _workspaceService = workspaceService;
            BindingContext = homeViewModel;

            _notificationService.OnShowNotification += async (msg, isError) =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await NotificationToast.ShowToast(msg, isError);
                });
            };

            _viewModel.RequestShowCreateTeam += OnRequestShowCreateTeam;
            _viewModel.RequestShowCreateBoard += OnRequestShowCreateBoard;
            _teamService = teamService;
            _sessionService = sessionService;
            _boardService = boardService;
        }

        private void OnRequestShowCreateTeam()
        {
            var viewModel = new CreateTeamPopupViewModel(_workspaceService, _notificationService, _teamService, _sessionService);
            var popup = new CreateTeamPopup();
            popup.BindingContext = viewModel;

            viewModel.RequestClose = () => popup.Close();

            this.ShowPopup(popup);
        }

        private void OnRequestShowCreateBoard()
        {

            var selectedTeamModel = _viewModel.GetSelectedTeamModel(); 

            var viewModel = new CreateBoardPopupViewModel(
                _workspaceService,
                _notificationService,
                _boardService,
                selectedTeamModel);

            var popup = new CreateBoardPopup();
            popup.BindingContext = viewModel;

            viewModel.RequestClose = () => popup.Close();

            this.ShowPopup(popup);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.RequestShowCreateTeam -= OnRequestShowCreateTeam;
            _viewModel.RequestShowCreateBoard -= OnRequestShowCreateBoard;

            _viewModel.Unsubscribe();
        }
    }
}