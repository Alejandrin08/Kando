using kando_desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services
{
    public class WorkspaceService
    {
        public ObservableCollection<Team> Teams { get; } = new();
        public ObservableCollection<Board> Boards { get; } = new();

        public void AddTeam(Team team)
        {
            Teams.Add(team);
        }

        public void AddBoard(Board board)
        {
            Boards.Add(board);
        }
    }
}
