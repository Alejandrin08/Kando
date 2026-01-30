using kando_backend.DTOs.Requests;
using kando_backend.Models;
using kando_backend.Services.Implementations;
using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace kando_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

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
    }
}
