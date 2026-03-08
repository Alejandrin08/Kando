using kando_backend.DTOs.Responses;

namespace kando_backend.Services.Interfaces
{
    public interface ITeamMemberService
    {
        Task<bool> RemoveMemberAsync(int teamId, int userIdToRemove, int ownerId);
        Task<List<TeamMemberDto>> GetTeamMembersAsync(int teamId, int ownerId);
    }
}