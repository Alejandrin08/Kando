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
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string icon;

        [ObservableProperty]
        private Color teamColor;

        [ObservableProperty]
        private int numberBoards;

        public int MemberCount { get; set; }
        public List<Member> Members { get; set; } = new List<Member>();
    }
}
