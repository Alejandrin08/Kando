using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Helpers;
using kando_backend.Models;
using kando_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace kando_backend.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly KandoDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(KandoDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            string token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                UserIcon = user.UserIcon,
                Token = token
            };
        }

        public async Task<string?> GenerateRecoveryCodeAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var existsUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);
            if (existsUser == null) return null;

            string code = GenerateRecoveryCode();

            existsUser.RecoveryCode = code;
            existsUser.CodeCreatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            string htmlBody = EmailTemplates.GetRecoveryCodeTemplate(code);
            string subject = "Kando: Recuperación de contraseña";
            await _emailService.SendEmailAsync(existsUser.Email, subject, htmlBody);
            return code;
        }

        public async Task<bool> ValidateRecoveryCodeAsync(ValidateCodeDto validateCodeDto)
        {
            var existsUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == validateCodeDto.Email);

            if (existsUser == null || existsUser.RecoveryCode != validateCodeDto.Code || existsUser.CodeCreatedAt == null)
                return false;

            DateTime expirationDate = existsUser.CodeCreatedAt.Value.AddMinutes(15);

            if (DateTime.UtcNow > expirationDate)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var existsUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == resetPasswordDto.Email);

            if (existsUser == null || existsUser.RecoveryCode != resetPasswordDto.Code || existsUser.CodeCreatedAt == null)
                return false;

            DateTime expirationDate = existsUser.CodeCreatedAt.Value.AddMinutes(15);
            if (DateTime.UtcNow > expirationDate) return false;

            existsUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);

            existsUser.RecoveryCode = null;
            existsUser.CodeCreatedAt = null;

            await _context.SaveChangesAsync();

            return true;
        }

        private string GenerateRecoveryCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GenerateJwtToken(Models.User user)
        {
            var keyString = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(keyString)) throw new Exception("La clave JWT no está configurada en appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim("user_icon", user.UserIcon)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
