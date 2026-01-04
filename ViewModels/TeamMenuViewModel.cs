using CommunityToolkit.Maui.Views;
using kando_desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.ViewModels
{
    public class TeamMenuViewModel
    {
        private readonly Team _team;
        private readonly HomeViewModel _mainViewModel;
        private readonly Popup _popup;

        public Command EditTeamCommand { get; }
        public Command DeleteTeamCommand { get; }

        public TeamMenuViewModel(Team team, HomeViewModel mainVm, Popup popup)
        {
            _team = team;
            _mainViewModel = mainVm;
            _popup = popup;

            EditTeamCommand = new Command(() =>
            {
                _popup.Close();
                // Aquí llamarías a tu lógica de edición
                Console.WriteLine($"Editar {_team.Name}");
            });

            DeleteTeamCommand = new Command(() =>
            {
                _popup.Close();

                // Llamamos al método borrar del ViewModel principal
                // (Necesitarás crear este método en HomeViewModel)
                // _mainViewModel.DeleteTeam(_team); 
                Console.WriteLine($"Eliminar {_team.Name}");
            });
        }
    }   
}
