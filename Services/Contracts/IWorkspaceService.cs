using kando_desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Contracts
{
    public interface IWorkspaceService
    {
        ObservableCollection<Team> Teams { get; }
        ObservableCollection<Board> Boards { get; }
        ObservableCollection<Member> Members { get; }

        void CreateTeam(string name, string iconSource, Color teamColor);
        void CreateBoard(string name, string iconSource, Team team);
        void UpdateTeam(Team team, string newName, string newIconSource, Color newTeamColor);
        void DeleteTeam(Team team);
        void DeleteMemberTeam(Member member, Team team);
        void DeleteBoard(Board board);
    }
}
