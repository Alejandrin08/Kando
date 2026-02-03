using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Models;
using kando_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace kando_backend.Services.Implementations
{
    public class BoardService : IBoardService
    {

        private readonly KandoDbContext _context;

        public BoardService(KandoDbContext context)
        {
            _context = context;
        }

        public async Task<BoardResponseDto> CreateBoardAsync(CreateBoardDto createBoardDto, int ownerId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == createBoardDto.TeamId && t.OwnerId == ownerId);

            if (team == null)
            {
                throw new Exception("Team not found or you do not have permission to add a board to this team.");
            }

            var board = new Board
            {
                Name = createBoardDto.Name,
                Icon = createBoardDto.Icon,
                TeamId = createBoardDto.TeamId,
                CreatedAt = DateTime.UtcNow,

                BoardLists = new List<BoardList>
                {
                    new BoardList { Name = "Por hacer", Position = 0 },
                    new BoardList { Name = "En proceso", Position = 1 },
                    new BoardList { Name = "Terminado", Position = 2 }
                }
            };

            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return new BoardResponseDto
            {
                Id = board.Id,
                Name = board.Name,
                Icon = board.Icon,
                TeamId = board.TeamId,
                CreatedAt = board.CreatedAt.GetValueOrDefault(DateTime.UtcNow),

                CompletedTasks = 0,
                TotalTasks = 0,
                TotalTaskPorcentage = 0
            };
        }

        public async Task<List<BoardResponseDto>> GetBoardsUserAsync(int ownerId)
        {
            var boards = await _context.Boards
                .Where(b => b.Team.OwnerId == ownerId && (b.Team.IsDeleted == false || b.Team.IsDeleted == null))
                .AsNoTracking()
                .Select(b => new BoardResponseDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Icon = b.Icon,
                    TeamId = b.TeamId,
                    CreatedAt = b.CreatedAt.GetValueOrDefault(DateTime.UtcNow),
                    TotalTasks = b.TotalTasks,
                    CompletedTasks = b.CompletedTasks, 
                    TotalTaskPorcentage = 0
                })
                .ToListAsync();

            foreach (var b in boards)
            {
                if (b.TotalTasks > 0)
                {
                    b.TotalTaskPorcentage = (double)b.CompletedTasks / b.TotalTasks;
                }
            }

            return boards;
        }
    }
}
