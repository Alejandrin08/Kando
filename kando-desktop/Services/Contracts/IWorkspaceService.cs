using kando_desktop.DTOs.Requests;
using kando_desktop.DTOs.Responses;
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

        void CreateTeam(TeamResponseDto dto, UserSession currentUser);
        void CreateBoard(BoardResponseDto boardDto);
        void UpdateTeam(int teamId, UpdateTeamDto updateTeamDto);
        void DeleteTeam(Team team);
        void DeleteMemberTeam(Member member, Team team);
        void DeleteBoard(Board board);
        void ClearData();

        Task InitializeDataAsync();
        Task ForceRefreshAsync();
    }
}
