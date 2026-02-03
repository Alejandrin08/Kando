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
    public class BoardController : ControllerBase
    {

        private readonly IBoardService _boardService;

        public BoardController(IBoardService boardService)
        {
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

        [HttpPost]
        public async Task<IActionResult> CreateBoard([FromBody] CreateBoardDto createBoardDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            try
            {
                var createBoard = await _boardService.CreateBoardAsync(createBoardDto, userId.Value);

                return StatusCode(201, createBoard);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating the board." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyBoards()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });
            try
            {
                var boards = await _boardService.GetBoardsUserAsync(userId.Value);
                return Ok(boards);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the boards." });
            }
        }
    }
}
