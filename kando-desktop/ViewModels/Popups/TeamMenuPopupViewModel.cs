using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;

namespace kando_desktop.ViewModels.Popups
{
    public partial class TeamMenuPopupViewModel : ObservableObject
    {
        private readonly Team _team;

        public Action RequestClose;
        public Action<Team> RequestEditTeam;
        public Action<Team> RequestRemoveMember;
        public Action<Team> RequestDeleteTeam;

        public TeamMenuPopupViewModel(Team team)
        {
            _team = team;
        }

        [RelayCommand]
        private void EditTeam()
        {
            RequestClose?.Invoke();
            RequestEditTeam?.Invoke(_team);
        }

        [RelayCommand]
        private void RemoveMember()
        {
            RequestClose?.Invoke();
            RequestRemoveMember?.Invoke(_team);
        }

        [RelayCommand]
        private void DeleteTeam()
        {
            RequestClose?.Invoke();
            RequestDeleteTeam?.Invoke(_team);
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke();
        }
    }
}