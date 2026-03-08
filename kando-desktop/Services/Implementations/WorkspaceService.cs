using CommunityToolkit.Mvvm.ComponentModel;
using kando_desktop.DTOs.Requests;
using kando_desktop.DTOs.Responses;
using kando_desktop.Enums;
using kando_desktop.Helpers;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using System.Collections.ObjectModel;

namespace kando_desktop.Services.Implementations
{
    public class WorkspaceService : ObservableObject, IWorkspaceService
    {
        private ObservableCollection<Team> _teams = new();
        public ObservableCollection<Team> Teams
        {
            get => _teams;
            set => SetProperty(ref _teams, value);
        }

        private ObservableCollection<Board> _boards = new();
        public ObservableCollection<Board> Boards
        {
            get => _boards;
            set => SetProperty(ref _boards, value);
        }

        public ObservableCollection<Member> Members { get; } = new();

        private readonly ITeamService _teamService;
        private readonly ISessionService _sessionService;
        private readonly IBoardService _boardService;
        private readonly IDashboardService _dashboardService;

        private bool _isDataLoaded = false;

        public WorkspaceService(
            ITeamService teamService,
            ISessionService sessionService,
            IBoardService boardService,
            IDashboardService dashboardService)
        {
            _teamService = teamService;
            _sessionService = sessionService;
            _boardService = boardService;
            _dashboardService = dashboardService;
        }

        public async Task InitializeDataAsync()
        {
            if (_isDataLoaded) return;
            await ForceRefreshAsync();
        }

        public async Task ForceRefreshAsync()
        {
            var dashboardData = await _dashboardService.GetDashboardAsync();
            if (dashboardData == null) return;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SyncTeams(dashboardData.Teams);
                SyncBoards(dashboardData.Boards);
                OnPropertyChanged("ForceUIRefresh");
            });

            _isDataLoaded = true;

            await SubscribeToAllTeamsAsync();
        }

        private async Task SubscribeToAllTeamsAsync()
        {
            var notificationService = ServiceHelper.GetService<INotificationService>();
            if (notificationService != null && notificationService.IsConnected)
            {
                var teamIds = Teams.Select(t => t.Id).ToList();
                if (teamIds.Any())
                {
                    await notificationService.SubscribeToTeamsAsync(teamIds);
                }
            }
        }

        private void SyncTeams(List<TeamResponseDto> teamDtos)
        {
            RemoveDeletedTeams(teamDtos);

            foreach (var dto in teamDtos)
            {
                var existingTeam = Teams.FirstOrDefault(t => t.Id == dto.Id);

                if (existingTeam != null)
                {
                    UpdateExistingTeam(existingTeam, dto);
                }
                else
                {
                    AddNewTeam(dto);
                }
            }
        }

        private void RemoveDeletedTeams(List<TeamResponseDto> teamDtos)
        {
            var teamsToRemove = Teams
                .Where(t => !teamDtos.Any(dto => dto.Id == t.Id))
                .ToList();

            foreach (var team in teamsToRemove)
            {
                Teams.Remove(team);
            }
        }

        private void UpdateExistingTeam(Team team, TeamResponseDto dto)
        {
            var teamColor = ParseColor(dto.Color);

            team.Name = dto.Name;
            team.Icon = dto.Icon;
            team.TeamColor = teamColor;
            team.IsCurrentUserOwner = dto.IsCurrentUserOwner;
            team.TotalCapacity = dto.TotalCapacity;

            UpdateTeamMembers(team, dto.Members, teamColor);
            team.MemberCount = team.Members.Count;
        }

        private void AddNewTeam(TeamResponseDto dto)
        {
            var teamColor = ParseColor(dto.Color);

            var newTeam = new Team
            {
                Id = dto.Id,
                Name = dto.Name,
                Icon = dto.Icon,
                TeamColor = teamColor,
                IsCurrentUserOwner = dto.IsCurrentUserOwner,
                TotalCapacity = dto.TotalCapacity,
                Members = new ObservableCollection<Member>()
            };

            UpdateTeamMembers(newTeam, dto.Members, teamColor);
            newTeam.MemberCount = newTeam.Members.Count;

            Teams.Add(newTeam);
        }

        private void UpdateTeamMembers(Team team, List<TeamMemberDto> memberDtos, Color teamColor)
        {
            team.Members.Clear();

            foreach (var memberDto in memberDtos)
            {
                TeamRole assignedRole = TeamRole.Member;
                if (memberDto.Role == "Owner")
                    assignedRole = TeamRole.Owner;
                else if (memberDto.Status == "Pending")
                    assignedRole = TeamRole.Pending;

                team.Members.Add(new Member
                {
                    UserId = memberDto.UserId,
                    Name = memberDto.Name,
                    Initials = GetInitials(memberDto.Name),
                    BaseColor = teamColor,
                    Role = assignedRole
                });
            }
        }

        private void SyncBoards(List<BoardResponseDto> boardDtos)
        {
            Boards.Clear();

            foreach (var boardDto in boardDtos)
            {
                var team = Teams.FirstOrDefault(t => t.Id == boardDto.TeamId);
                if (team == null) continue;

                AddBoardToCollection(boardDto, team);
                IncrementTeamBoardCount(team);
            }
        }

        private void AddBoardToCollection(BoardResponseDto boardDto, Team team)
        {
            var board = new Board
            {
                Id = boardDto.Id,
                Name = boardDto.Name,
                Icon = boardDto.Icon,
                TeamName = team,
                TeamColor = team.TeamColor,
                CompletedTasks = boardDto.CompletedTasks,
                TotalTasks = boardDto.TotalTasks,
                TotalTaskPorcentage = boardDto.TotalTaskPorcentage
            };

            Boards.Add(board);
        }

        private void IncrementTeamBoardCount(Team team)
        {
            if (team.NumberBoards == 0)
                team.NumberBoards = 1;
            else
                team.NumberBoards++;
        }

        private Color ParseColor(string colorString)
        {
            try
            {
                return Color.FromArgb(colorString);
            }
            catch
            {
                return Color.FromHex("#8f45ef");
            }
        }

        public void ClearData()
        {
            Teams = new ObservableCollection<Team>();
            Boards = new ObservableCollection<Board>();
            Members.Clear();
            _isDataLoaded = false;
        }

        public async void CreateTeam(TeamResponseDto dto, UserSession currentUser)
        {
            string userName = currentUser?.UserName ?? "Tú";
            var teamColor = ParseColor(dto.Color);

            var myself = new Member
            {
                UserId = currentUser?.UserId ?? 0,
                Initials = GetInitials(userName),
                Name = userName,
                BaseColor = teamColor,
                Role = TeamRole.Owner
            };

            var newTeam = new Team
            {
                Id = dto.Id,
                Name = dto.Name,
                Icon = dto.Icon,
                TeamColor = teamColor,
                MemberCount = 1,
                NumberBoards = 0,
                TotalCapacity = 1,
                IsCurrentUserOwner = dto.IsCurrentUserOwner,
                Members = new ObservableCollection<Member> { myself }
            };

            Teams.Insert(0, newTeam);

            var notificationService = ServiceHelper.GetService<INotificationService>();
            if (notificationService != null && notificationService.IsConnected)
            {
                await notificationService.SubscribeToTeamsAsync(new List<int> { dto.Id });
            }
        }

        public void CreateBoard(BoardResponseDto boardDto)
        {
            var parentTeam = Teams.FirstOrDefault(t => t.Id == boardDto.TeamId);
            if (parentTeam == null) return;

            var newBoard = new Board
            {
                Id = boardDto.Id,
                Name = boardDto.Name,
                Icon = boardDto.Icon,
                TeamName = parentTeam,
                TeamColor = parentTeam.TeamColor,
                CompletedTasks = boardDto.CompletedTasks,
                TotalTasks = boardDto.TotalTasks,
                TotalTaskPorcentage = boardDto.TotalTaskPorcentage
            };

            Boards.Add(newBoard);
            parentTeam.NumberBoards++;
        }

        public void UpdateTeam(int teamId, UpdateTeamDto updateTeamDto)
        {
            var team = Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null) return;

            team.Name = updateTeamDto.Name;
            team.Icon = updateTeamDto.Icon;
            team.TeamColor = ParseColor(updateTeamDto.Color);

            foreach (var member in team.Members)
            {
                member.BaseColor = team.TeamColor;
            }
        }

        public void UpdateBoard(int boardId, UpdateBoardDto updateBoardDto)
        {
            var board = Boards.FirstOrDefault(b => b.Id == boardId);
            if (board == null) return;

            board.Name = updateBoardDto.Name;
            board.Icon = updateBoardDto.Icon;
        }

        public void DeleteTeam(Team team)
        {
            if (team == null) return;

            var associatedBoards = Boards
                .Where(b => b.TeamName == team)
                .ToList();

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

            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper();

            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
    }
}