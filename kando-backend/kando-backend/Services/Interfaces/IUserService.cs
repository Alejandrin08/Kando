using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;

namespace kando_backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDetails> GetUserByEmail(string email);
    }
}
