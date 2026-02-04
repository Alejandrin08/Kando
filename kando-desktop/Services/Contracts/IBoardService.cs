using kando_desktop.DTOs.Requests;
using kando_desktop.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Services.Contracts
{
    public interface IBoardService
    {
        Task<BoardResponseDto?> CreateBoardAsync(CreateBoardDto createBoardDto);

        Task<List<BoardResponseDto>> GetMyBoardsAsync();

        Task<bool> DeleteBoardAsync(int boardId);
    }
}
