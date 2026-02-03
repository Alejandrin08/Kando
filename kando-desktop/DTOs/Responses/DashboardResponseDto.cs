using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.DTOs.Responses
{
    public class DashboardResponseDto
    {
        public List<TeamResponseDto> Teams { get; set; } = new();
        public List<BoardResponseDto> Boards { get; set; } = new();
    }
}
