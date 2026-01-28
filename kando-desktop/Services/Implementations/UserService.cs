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
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateUserAsync(CreateUserDto createUserDto)
        {
            var response = await _httpClient.PostAsJsonAsync("user", createUserDto);

            if (!response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }
            return true;
        }
    }
}
