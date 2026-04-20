using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;

namespace kando_backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto request);
        Task<string?> GenerateRecoveryCodeAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ValidateRecoveryCodeAsync(ValidateCodeDto validateCodeDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
