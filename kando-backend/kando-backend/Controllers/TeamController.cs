using kando_backend.DTOs.Requests;
using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace kando_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {

        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
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

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto createTeamDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            try
            {
                var createdTeam = await _teamService.CreateTeamAsync(createTeamDto, userId.Value);

                return StatusCode(201, createdTeam);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal error while creating the team." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTeams()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            try
            {
                var teams = await _teamService.GetTeamsUserAsync(userId.Value);
                return Ok(teams);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal error while retrieving teams." });
            }
        }

        [HttpPut("{teamId}")]
        public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] UpdateTeamDto updateTeamDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });
            try
            {
                var updated = await _teamService.UpdateTeamAsync(teamId, updateTeamDto, userId.Value);
                if (!updated)
                {
                    return NotFound(new { message = "Team not found or you do not have permission to update it." });
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal error while updating the team." });
            }
        }
    }
}
