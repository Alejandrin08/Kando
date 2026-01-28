using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Models
{
    public partial class User : ObservableObject
    {
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string useremail;

        [ObservableProperty]
        private string usercolor;

        [ObservableProperty]
        private string userinitials;
    }
}
