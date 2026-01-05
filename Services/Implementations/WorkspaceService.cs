using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using System.Collections.ObjectModel;

namespace kando_desktop.Services
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

        public void DeleteMemberTeam(Member member, Team team)
        {
            if (team.Members.Contains(member))
            {
                team.Members.Remove(member);
                team.MemberCount--;
            }
        }
    }
}