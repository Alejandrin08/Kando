using CommunityToolkit.Maui.Views;
using kando_desktop.ViewModels;

namespace kando_desktop.Views.Popups;

public partial class ProfileMenuPopup : Popup
{
    public ProfileMenuPopup(BaseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnItemClicked(object sender, EventArgs e)
    {
        Close();
    }
}