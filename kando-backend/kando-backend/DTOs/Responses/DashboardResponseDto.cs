namespace kando_backend.DTOs.Responses
{
    public class DashboardResponseDto
    {
        public List<TeamResponseDto> Teams { get; set; }
        public List<BoardResponseDto> Boards { get; set; }
    }
}
