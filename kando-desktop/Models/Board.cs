using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public partial class Board : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private ImageSource icon;

        [ObservableProperty]
        private Team teamName;

        [ObservableProperty]
        private Color teamColor;

        [ObservableProperty]
        private int totalTasks;

        [ObservableProperty]
        private int taskCount;

        [ObservableProperty]
        private int totalTaskPorcentage;
    }
}
