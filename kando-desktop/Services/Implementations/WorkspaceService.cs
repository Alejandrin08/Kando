using kando_desktop.DTOs.Requests;
using kando_desktop.Enums;
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

        public void CreateTeam(CreateTeamDto dto, UserSession currentUser)
        {
            string userName = currentUser?.UserName ?? "Tú";
            string initials = GetInitials(userName);

            var myself = new Member
            {
                Initials = initials,
                Name = $"{userName} (Admin)",
                BaseColor = Color.FromArgb(dto.Color), 
                Role = Enums.TeamRole.Owner
            };

            var newTeam = new Team
            {
                Name = dto.Name,
                Icon = dto.Icon,
                TeamColor = Color.FromArgb(dto.Color),
                MemberCount = 1,
                NumberBoards = 0,
                Members = new ObservableCollection<Member> { myself }
            };

            Teams.Insert(0, newTeam);
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

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "YO";
            var parts = name.Trim().Split(' ');
            if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
    }
}