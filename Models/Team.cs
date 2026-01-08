using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        [ObservableProperty]
        private ObservableCollection<Member> members = new ObservableCollection<Member>();

        public int MemberCount { get; set; }
    }
}
