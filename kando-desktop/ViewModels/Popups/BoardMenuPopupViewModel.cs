using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using System.IO;

namespace kando_desktop.ViewModels.Popups
{
    public partial class BoardMenuPopupViewModel : ObservableObject
    {

        private readonly Board _board;

        public Action RequestClose;
        public Action<Board> RequestEditBoard;
        public Action<Board> RequestDeleteBoard;

        public BoardMenuPopupViewModel(Board board)
        {
            _board = board;
        }

        [RelayCommand]
        private void EditTeam()
        {
            RequestClose?.Invoke();
            RequestEditBoard?.Invoke(_board);
        }


        [RelayCommand]
        private void DeleteBoard()
        {
            RequestClose?.Invoke();
            RequestDeleteBoard?.Invoke(_board);
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke();
        }

    }
}
