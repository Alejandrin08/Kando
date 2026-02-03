using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.DTOs.Responses
{
    public class BoardResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int TeamId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TaskCount { get; set; }
        public int TotalTasks { get; set; } 
        public double TotalTaskPorcentage { get; set; }
    }
}
