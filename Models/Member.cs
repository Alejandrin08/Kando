using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public partial class Member : ObservableObject
    {
        public required string Initials { get; set; }

        [ObservableProperty]
        private Color baseColor;
    }
}
