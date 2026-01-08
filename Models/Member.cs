using CommunityToolkit.Mvvm.ComponentModel;
using kando_desktop.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public partial class Member : ObservableObject
    {
        [ObservableProperty] 
        private string initials;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private Color baseColor;

        [ObservableProperty]
        private TeamRole role;
    }
}
