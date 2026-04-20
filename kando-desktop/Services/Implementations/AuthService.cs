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
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> GenerateRecoveryCodeAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/forgot-password", forgotPasswordDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ValidateRecoveryCodeAsync(ValidateCodeDto validateCodeDto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/validate-code", validateCodeDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"auth/reset-password", resetPasswordDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error API: {errorContent}");
                return false;
            }
            return true;
        }

        public async Task<LoginResponseDto> LoginAsync(string email, string password)
        {
            var loginData = new LoginDto
            {
                Email = email,
                Password = password
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Login: {ex.Message}");
                return null;
            }
        }
    }
}
