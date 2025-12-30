using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public class Team
    {
        public required string Name { get; set; }
        public required ImageSource Icon { get; set; }
        public int MemberCount { get; set; }
        public int NumberBoards { get; set; }
        public required Color TeamColor { get; set; }
        public List<Member> Members { get; set; } = new List<Member>();
    }
}
