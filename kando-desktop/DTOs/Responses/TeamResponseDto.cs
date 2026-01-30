using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.DTOs.Responses
{
    public class TeamResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
