using CommunityToolkit.Mvvm.Messaging.Messages;
using kando_desktop.Models;

namespace kando_desktop.Helpers
{
    public class BoardChangedMessage : ValueChangedMessage<Board>
    {
        public BoardChangedMessage(Board board) : base(board)
        {
        }
    }
}