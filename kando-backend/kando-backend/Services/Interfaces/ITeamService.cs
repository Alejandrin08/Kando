using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Models;

namespace kando_backend.Services.Interfaces
{
    public interface ITeamService
    {
        Task<TeamResponseDto> CreateTeamAsync(CreateTeamDto createTeamDto, int ownerId);

        Task<List<TeamResponseDto>> GetTeamsUserAsync(int ownerId);

        Task<bool> UpdateTeamAsync(int teamId, UpdateTeamDto updateTeamDto, int ownerId);

        Task<bool> DeleteTeamAsync(int teamId, int ownerId);

        Task<bool> InviteMemberAsync(int teamId, string emailToInvite, int ownerId);

        Task<bool> UpdateInvitationAsync(int teamId, int userId, UpdateInvitationDecisionDto dto);
    }
}
