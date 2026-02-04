using kando_desktop.DTOs.Requests;
using kando_desktop.DTOs.Responses;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        public async Task<List<TeamResponseDto>> GetMyTeamsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("team");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TeamResponseDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            return new List<TeamResponseDto>();
        }

        public async Task<TeamResponseDto?> CreateTeamAsync(CreateTeamDto createTeamDto)
        {

            var response = await _httpClient.PostAsJsonAsync("team", createTeamDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error API: {errorContent}");
                return null;
            }
            return await response.Content.ReadFromJsonAsync<TeamResponseDto>();
        }

        public async Task<bool> UpdateTeamAsync(int teamId, UpdateTeamDto updateTeamDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"team/{teamId}", updateTeamDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error API: {errorContent}");
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteTeamAsync(int teamId)
        {
            var response = await _httpClient.DeleteAsync($"team/{teamId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error API: {errorContent}");
                return false;
            }

            return true;
        }
    }
}
