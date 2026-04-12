using kando_desktop.DTOs.Requests;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ISessionService _sessionService;

        public UserService(HttpClient httpClient, ISessionService sessionService)
        {
            _httpClient = httpClient;
            _sessionService = sessionService;
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

        public async Task<bool> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var currentUser = _sessionService.CurrentUser;
            var userId = currentUser.UserId;
            var response = await _httpClient.PutAsJsonAsync($"user/{userId}", updateUserDto);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new HttpRequestException("Email taken", null, System.Net.HttpStatusCode.Conflict);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error API: {errorContent}");
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateUserPasswordAsync(UpdatePasswordDto updatePasswordDto)
        {
            var currentUser = _sessionService.CurrentUser;
            var userId = currentUser.UserId;

            var response = await _httpClient.PutAsJsonAsync($"user/password/{userId}", updatePasswordDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        if (errorContent.Contains("different", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new HttpRequestException(AppResources.SamePasswordError, null, HttpStatusCode.BadRequest);
                        }

                        if (errorContent.Contains("incorrect", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new HttpRequestException(AppResources.WrongCurrentPassword, null, HttpStatusCode.BadRequest);
                        }

                        if (errorContent.Contains("match", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new HttpRequestException(AppResources.PasswordsDoNotMatch, null, HttpStatusCode.BadRequest);
                        }
                    }
                }

                return false;
            }

            return true;
        }
    }
}
