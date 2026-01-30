using kando_desktop.DTOs.Requests;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Implementations
{
    public class TeamService : ITeamService
    {

        private readonly HttpClient _httpClient;

        public TeamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateTeamAsync(CreateTeamDto createTeamDto)
        {
            var response = await _httpClient.PostAsJsonAsync("team", createTeamDto);

            if (!response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }
            return true;
        }
    }
}
