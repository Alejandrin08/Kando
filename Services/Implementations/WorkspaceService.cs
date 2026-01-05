using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using System.Collections.ObjectModel;

namespace kando_desktop.Services.Implementations
{
    public class WorkspaceService : IWorkspaceService
    {
        public ObservableCollection<Team> Teams { get; } = new();
        public ObservableCollection<Board> Boards { get; } = new();
        public ObservableCollection<Member> Members { get; } = new();

        public void CreateTeam(string name, string iconSource, Color teamColor)
        {
            var myself = new Member
            {
                Initials = "YO",
                BaseColor = teamColor
            };

            var newTeam = new Team
            {
                Name = name,
                Icon = iconSource,
                TeamColor = teamColor,
                MemberCount = 1,
                NumberBoards = 0,
                Members = new List<Member> { myself }
            };

            Teams.Add(newTeam);
        }

        public void CreateBoard(string name, string iconSource, Team team)
        {
            var newBoard = new Board
            {
                Name = name,
                Icon = iconSource,
                TeamName = team,
                TeamColor = team.TeamColor,
                TaskCount = 0,
                TotalTasks = 0,
                TotalTaskPorcentage = 0
            };

            Boards.Add(newBoard);

            team.NumberBoards++;
        }

        public void UpdateTeam(Team team, string newName, string newIconSource, Color newTeamColor)
        {
            if (team == null) return;

            team.Name = newName;
            team.Icon = newIconSource;
            team.TeamColor = newTeamColor;

            var associatedBoards = Boards.Where(b => b.TeamName == team);
            foreach (var board in associatedBoards)
            {
                board.TeamColor = newTeamColor;
            }

            if (team.Members != null)
            {
                foreach (var member in team.Members)
                {
                    member.BaseColor = newTeamColor;
                }
            }
        }

        public void DeleteTeam(Team team)
        {
            if (team == null) return;

            var associatedBoards = Boards.Where(b => b.TeamName == team).ToList();
            foreach (var board in associatedBoards)
            {
                Boards.Remove(board);
            }

            Teams.Remove(team);
        }

        public void DeleteMemberTeam(Member member, Team team)
        {
            if (team?.Members == null || member == null) return;

            if (team.Members.Contains(member))
            {
                team.Members.Remove(member);
                team.MemberCount--;
            }
        }

        public void DeleteBoard(Board board)
        {
            if (board == null) return;

            if (board.TeamName != null)
            {
                board.TeamName.NumberBoards--;
            }

            Boards.Remove(board);
        }
    }
}