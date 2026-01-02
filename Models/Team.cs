using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public partial class Team : ObservableObject
    {
        public required string Name { get; set; }
        public required ImageSource Icon { get; set; }
        public int MemberCount { get; set; }

        [ObservableProperty]
        private int numberBoards;
        public required Color TeamColor { get; set; }
        public List<Member> Members { get; set; } = new List<Member>();
    }
}
