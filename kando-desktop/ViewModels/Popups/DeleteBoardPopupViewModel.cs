using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.ViewModels.Popups
{
    public partial class DeleteBoardPopupViewModel : ObservableObject
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;
        private readonly IBoardService _boardService;
        private readonly Board _boardToDelete;

        [ObservableProperty]
        private bool isBusy;

        public Action RequestClose;

        public DeleteBoardPopupViewModel(IWorkspaceService workspaceService, INotificationService notificationService, IBoardService boardService, Board boardToDelete)
        {
            _workspaceService = workspaceService;
            _notificationService = notificationService;
            _boardService = boardService;
            _boardToDelete = boardToDelete;
        }

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke();


        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task DeleteBoard()
        {
            IsBusy = true;
            try
            {
                var success = await _boardService.DeleteBoardAsync(_boardToDelete.Id);

                if (success)
                {
                    _workspaceService.DeleteBoard(_boardToDelete);
                    RequestClose?.Invoke();
                    _notificationService.Show(AppResources.BoardDeleteSuccessfully);
                }
                else
                {
                    _notificationService.Show(AppResources.FailedToDeleteBoard, true);
                }

            }
            catch (Exception)
            {
                _notificationService.Show(AppResources.UnexpectedError);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
