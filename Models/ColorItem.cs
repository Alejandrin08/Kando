using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public partial class ColorItem : ObservableObject
    {
        [ObservableProperty]
        private string colorHex;

        [ObservableProperty]
        private bool isSelected;

        public Color Color => Color.FromArgb(ColorHex);
    }
}
