using kando_desktop.DTOs.Requests;
using kando_desktop.DTOs.Responses;

namespace kando_desktop.Services.Contracts
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(string email, string password);
        Task<bool> GenerateRecoveryCodeAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ValidateRecoveryCodeAsync(ValidateCodeDto validateCodeDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
