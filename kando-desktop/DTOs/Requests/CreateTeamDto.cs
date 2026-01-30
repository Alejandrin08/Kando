using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.DTOs.Requests
{
    public class CreateTeamDto
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }
}
