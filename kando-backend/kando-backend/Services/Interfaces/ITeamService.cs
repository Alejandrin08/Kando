using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Models;

namespace kando_backend.Services.Interfaces
{
    public interface ITeamService
    {
        Task<Team> CreateTeamAsync(CreateTeamDto createTeamDto, int ownerId);

        Task<List<TeamResponseDto>> GetTeamsUserAsync(int ownerId);  
    }
}
