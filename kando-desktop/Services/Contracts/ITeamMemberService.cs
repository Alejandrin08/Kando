using kando_desktop.DTOs.Responses;

namespace kando_desktop.Services.Contracts
{
    public interface ITeamMemberService
    {
        Task<bool> RemoveMemberAsync(int teamId, int userIdToRemove);

        Task<List<TeamMemberDto>> GetMembersAsync(int teamId);
    }
}