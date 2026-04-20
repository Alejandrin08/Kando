using kando_backend.DTOs.Requests;
using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace kando_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            await _authService.GenerateRecoveryCodeAsync(request);

            return Ok(new { message = "If the email is registered, a recovery code will be sent." });
        }

        [HttpPost("validate-code")]
        public async Task<IActionResult> ValidateCode([FromBody] ValidateCodeDto validateCodeDto)
        {
            var isValid = await _authService.ValidateRecoveryCodeAsync(validateCodeDto);
            if (!isValid)
            {
                return BadRequest(new { message = "Invalid or expired recovery code." });
            }

            return Ok(new { message = "Code validated successfully." });
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var isReset = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!isReset)
            {
                return BadRequest(new { message = "Error resetting password. The code might have expired." });
            }

            return Ok(new { message = "Password has been successfully reset." });
        }
    }
}