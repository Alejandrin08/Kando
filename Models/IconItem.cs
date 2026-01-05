using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public partial class IconItem : ObservableObject
    {
        [ObservableProperty]
        private string source;

        [ObservableProperty]
        private bool isSelected;
    }
}
