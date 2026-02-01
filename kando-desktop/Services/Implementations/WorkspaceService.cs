using kando_desktop.DTOs.Requests;
using kando_desktop.DTOs.Responses;
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


        private readonly ITeamService _teamService;
        private readonly ISessionService _sessionService;

        private bool _isDataLoaded = false;

        public WorkspaceService(ITeamService teamService, ISessionService sessionService)
        {
            _teamService = teamService;
            _sessionService = sessionService;
        }

        public async Task InitializeDataAsync()
        {
            if (_isDataLoaded) return;

            await ForceRefreshAsync();
        }

        public async Task ForceRefreshAsync()
        {
            var teamsDtos = await _teamService.GetMyTeamsAsync();

            Teams.Clear();

            var currentUser = _sessionService.CurrentUser;
            var userName = currentUser?.UserName;
            var initials = GetInitials(userName);

            foreach (var dto in teamsDtos)
            {
                Color teamColor;
                try { teamColor = Color.FromArgb(dto.Color); }
                catch { teamColor = Color.FromHex("#8f45ef"); } 

                var ownerMember = new Member
                {
                    Name = userName,
                    Initials = initials,
                    BaseColor = teamColor,
                    Role = TeamRole.Owner
                };

                var team = new Team
                {
                    Id = dto.Id, 
                    Name = dto.Name,
                    Icon = dto.Icon,
                    TeamColor = teamColor,
                    MemberCount = 1,
                    Members = new ObservableCollection<Member> { ownerMember }
                };

                Teams.Add(team);
            }

            _isDataLoaded = true;
        }

        public void ClearData()
        {
            Teams.Clear();
            Boards.Clear();
            Members.Clear();
            _isDataLoaded = false; 
        }

        public void CreateTeam(TeamResponseDto dto, UserSession currentUser)
        {
            string userName = currentUser?.UserName ?? "Tú";
            string initials = GetInitials(userName);

            Color teamColor;
            try { teamColor = Color.FromArgb(dto.Color); }
            catch { teamColor = Color.FromArgb("#8f45ef"); }

            var myself = new Member
            {
                Initials = initials,
                Name = $"{userName} (Admin)",
                BaseColor = Color.FromArgb(dto.Color), 
                Role = Enums.TeamRole.Owner
            };

            var newTeam = new Team
            {
                Id = dto.Id,
                Name = dto.Name,
                Icon = dto.Icon,
                TeamColor = teamColor,  
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

        public void UpdateTeam(int teamId, UpdateTeamDto updateTeamDto)
        {
            var team = Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null) return;
            team.Name = updateTeamDto.Name;
            team.Icon = updateTeamDto.Icon;
            try
            {
                team.TeamColor = Color.FromArgb(updateTeamDto.Color);
            }
            catch
            {
                team.TeamColor = Color.FromHex("#8f45ef");
            }

            foreach (var member in team.Members)
            {
                member.BaseColor = team.TeamColor;
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