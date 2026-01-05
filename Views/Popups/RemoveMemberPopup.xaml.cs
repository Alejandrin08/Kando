using CommunityToolkit.Maui.Views;
using kando_desktop.Models;
using kando_desktop.ViewModels;

namespace kando_desktop.Views.Popups;

public partial class RemoveMemberPopup : Popup
{
    private HomeViewModel _viewModel;
    public Team TeamSelected { get; }
    public Member MemberSelected { get; }

    public RemoveMemberPopup(Team team, Member member, HomeViewModel viewModel) 
	{
		InitializeComponent();

        _viewModel = viewModel;
        TeamSelected = team;
        MemberSelected = member;

        this.BindingContext = this;
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }
}