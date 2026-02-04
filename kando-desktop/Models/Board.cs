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
        private int id;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string icon;

        [ObservableProperty]
        private Team teamName;

        [ObservableProperty]
        private Color teamColor;

        [ObservableProperty]
        private int totalTasks;

        [ObservableProperty]
        private int completedTasks;

        [ObservableProperty]
        private double totalTaskPorcentage;
    }
}
