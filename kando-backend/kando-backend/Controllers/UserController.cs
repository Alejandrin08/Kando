using System.Security.Claims;
using kando_backend.DTOs.Requests;
using kando_backend.Models;
using kando_backend.Services.Implementations;
using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kando_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            var existingUser = await _userService.GetUserByEmail(dto.UserEmail);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email already registered" });
            }

            bool created = await _userService.CreateUserAsync(dto);

            if (!created)
            {
                return StatusCode(500, new { message = "Error creating user" });
            }

            return CreatedAtAction(nameof(GetUserByEmail), new { email = dto.UserEmail }, created);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

        [HttpPut("password/{userId}")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null || currentUserId != userId)
                return Unauthorized(new { message = "User not identified or unauthorized." });

            try
            {
                var updated = await _userService.ChangePassword(userId, updatePasswordDto);
                if (!updated)
                {
                    return NotFound(new { message = "User not found." });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message == "Same_Password")
            {
                return BadRequest(new { message = "The new password must be different from the current one." });
            }
            catch (InvalidOperationException ex) when (ex.Message == "Wrong_Password")
            {
                return BadRequest(new { message = "The current password is incorrect." });
            }
            catch (InvalidOperationException ex) when (ex.Message == "Passwords_Do_Not_Match")
            {
                return BadRequest(new { message = "The password do not match." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal error while updating the password." });
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null || currentUserId != userId)
                return Unauthorized(new { message = "User not identified or unauthorized." });

            try
            {
                var updated = await _userService.EditUserAsync(userId, updateUserDto);
                if (!updated)
                {
                    return NotFound(new { message = "User not found." });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message == "Email_Taken")
            {
                return Conflict(new { message = "Email is already registered by another user." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal error while updating the user." });
            }
        }
    }
}