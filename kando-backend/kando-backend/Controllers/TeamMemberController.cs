using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace kando_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamMemberController : ControllerBase
    {
        private readonly ITeamMemberService _teamMemberService;

        public TeamMemberController(ITeamMemberService teamMemberService)
        {
            _teamMemberService = teamMemberService;
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

        [HttpPut("{teamId}/{userIdToRemove}")]
        public async Task<IActionResult> RemoveMember(int teamId, int userIdToRemove)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            try
            {
                var updated = await _teamMemberService.RemoveMemberAsync(teamId, userIdToRemove, userId.Value);
                if (!updated)
                {
                    return NotFound(new { message = "Team Member not found, already removed, or you do not have permission." });
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal error while updating the team member." });
            }
        }

        [HttpGet("{teamId}/members")]
        public async Task<IActionResult> GetMembers(int teamId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var members = await _teamMemberService.GetTeamMembersAsync(teamId, userId.Value);
            return Ok(members);
        }
    }
}