using kando_desktop.DTOs.Requests;
using kando_desktop.DTOs.Responses;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Implementations
{
    public class BoardService : IBoardService
    {

        private readonly HttpClient _httpClient;

        public BoardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<BoardResponseDto?> CreateBoardAsync(CreateBoardDto createBoardDto)
        {
            var response = await _httpClient.PostAsJsonAsync("board", createBoardDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error API: {errorContent}");
                return null;
            }
            return await response.Content.ReadFromJsonAsync<BoardResponseDto>();
        }

        public async Task<bool> DeleteBoardAsync(int boardId)
        {
            var response = await _httpClient.DeleteAsync($"board/{boardId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error API: {errorContent}");
                return false;
            }

            return true;
        }

        public async Task<List<BoardResponseDto>> GetMyBoardsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("board");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<BoardResponseDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error API: {ex.Message}");
            }
            return new List<BoardResponseDto>();
        }
    }
}
