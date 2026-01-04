using CommunityToolkit.Maui.Views;
using kando_desktop.Models;
using kando_desktop.ViewModels;

namespace kando_desktop.Views.Popups;

public partial class TeamMenuPopup : Popup
{
    public TeamMenuPopup(Team team, HomeViewModel viewModel)
    {
        InitializeComponent();

        this.BindingContext = new TeamMenuViewModel(team, viewModel, this);
    }
}