using CommunityToolkit.Maui.Views;
using kando_desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input; 

namespace kando_desktop.ViewModels
{
    public class TeamMenuViewModel
    {
        public ICommand EditTeamCommand { get; }
        public ICommand RemoveMemberCommand { get; }

        public TeamMenuViewModel(Team team, HomeViewModel vm, Popup popup)
        {
            EditTeamCommand = new Command(async () =>
            {
                if (vm.EditTeamCommand.CanExecute(team))
                {
                    await vm.EditTeamCommand.ExecuteAsync(team);
                }
                popup.Close(); 
            });

            RemoveMemberCommand = new Command(() =>
            {
                if (vm.RemoveMemberCommand.CanExecute(team))
                {
                    vm.RemoveMemberCommand.Execute(team);
                }
                popup.Close();
            });
        }
    }
}