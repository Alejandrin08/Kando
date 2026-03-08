using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace kando_desktop.Models
{
    public partial class Team : ObservableObject
    {

        [ObservableProperty]
        private int id;

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

        [ObservableProperty]
        private bool isCurrentUserOwner;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanInvite))]
        private int totalCapacity;

        public bool CanInvite => IsCurrentUserOwner && TotalCapacity < 5;
    }
}
