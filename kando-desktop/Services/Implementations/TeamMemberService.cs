using kando_desktop.DTOs.Responses;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using System.Net.Http.Json;

namespace kando_desktop.Services.Implementations
{
    public class TeamMemberService : ITeamMemberService
    {

        private readonly HttpClient _httpClient;

        public TeamMemberService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TeamMemberDto>> GetMembersAsync(int teamId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"teammember/{teamId}/members");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TeamMemberDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            return new List<TeamMemberDto>();
        }

        public async Task<bool> RemoveMemberAsync(int teamId, int userIdToRemove)
        {
            var response = await _httpClient.PutAsync($"teammember/{teamId}/{userIdToRemove}", null);

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