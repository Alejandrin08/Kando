using kando_desktop.DTOs.Requests;
using kando_desktop.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Contracts
{
    public interface ITeamService
    {
        Task<bool> CreateTeamAsync(CreateTeamDto createTeamDto);

        Task<List<TeamResponseDto>> GetMyTeamsAsync();
    }
}
