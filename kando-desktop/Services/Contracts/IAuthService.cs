using kando_desktop.DTOs.Responses;

namespace kando_desktop.Services.Contracts
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(string email, string password);
    }
}
