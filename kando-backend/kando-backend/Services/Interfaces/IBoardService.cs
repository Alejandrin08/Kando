using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Models;

namespace kando_backend.Services.Interfaces
{
    public interface IBoardService
    {
        Task<BoardResponseDto> CreateBoardAsync(CreateBoardDto createBoardDto, int ownerId);

        Task<List<BoardResponseDto>> GetBoardsUserAsync(int ownerId);

        Task<bool> UpdateBoardAsync(int boardId, UpdateBoardDto updateBoardDto, int ownerId);

        Task<bool> DeleteBoardAsync(int boardId, int userId);
    }
}
