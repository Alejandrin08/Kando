using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public class Board
    {
        public required string Name { get; set; }
        public required ImageSource Icon { get; set; }
        public required Team TeamName { get; set; }
        public required Color TeamColor { get; set; }
        public int TotalTasks { get; set; }
        public int TaskCount { get; set; }
        public int TotalTaskPorcentage { get; set; }
    }
}
