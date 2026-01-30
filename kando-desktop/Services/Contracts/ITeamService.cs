using kando_desktop.DTOs.Requests;
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
    }
}
