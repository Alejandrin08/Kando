using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Models;
using kando_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace kando_backend.Services.Implementations
{
    public class UserService : IUserService
    {

        private readonly KandoDbContext _context;

        public UserService(KandoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUserAsync(CreateUserDto createUserDto)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == createUserDto.UserEmail);
            if (exists) return false;

            var user = new User
            {
                Username = createUserDto.UserName,
                Email = createUserDto.UserEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                UserIcon = "#8f45ef",
                CreatedAt = DateTime.UtcNow 
            };

            _context.Users.Add(user);

            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<UserDetails> GetUserByEmail(string email)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == email);  
            if (!exists) return null;

            var user = await _context.Users
                .Where(u => u.Email == email)
                .Select(u => new UserDetails
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    UserIcon = u.UserIcon
                })
                .FirstOrDefaultAsync();

            return user;
        }
    }
}
