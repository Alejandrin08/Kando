using kando_backend.DTOs.Responses;
using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace kando_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {

        private readonly ITeamService _teamService;
        private readonly IBoardService _boardService;

        public DashboardController(ITeamService teamService, IBoardService boardService)
        {
            _teamService = teamService;
            _boardService = boardService;
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

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            var teams = await _teamService.GetTeamsUserAsync(userId.Value);
            var boards = await _boardService.GetBoardsUserAsync(userId.Value);

            var response = new DashboardResponseDto
            {
                Teams = teams,
                Boards = boards
            };

            return Ok(response);
        }
    }
}
