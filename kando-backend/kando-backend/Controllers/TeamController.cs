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

        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            try
            {
                var deleted = await _teamService.DeleteTeamAsync(teamId, userId.Value);
                if (!deleted)
                {
                    return NotFound(new { message = "Team not found or you do not have permission to delete it." });
                }
                return NoContent();
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(new { message = ex.Message }); 
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal error while deleting the team." });
            }
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteMember([FromBody] InviteMemberRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _teamService.InviteMemberAsync(request.TeamId, request.EmailToInvite, userId.Value);
                return Ok(new { message = "Invitation sent." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); 
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); 
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
