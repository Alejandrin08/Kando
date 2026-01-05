using CommunityToolkit.Maui.Views;
using kando_desktop.Models;
using kando_desktop.ViewModels;
using Microsoft.Maui.Controls.Shapes; 

namespace kando_desktop.Views.Popups;

public partial class ModifyTeamPopup : Popup
{
    private HomeViewModel _viewModel;
    private Team _teamToEdit;
    private Member _memberToEdit;

    private string _selectedIconSource;
    private Color _selectedTeamColor;

    private Border _selectedIconBorder;
    private Border _selectedColorBorder;

    private Color _lightSelectionColor = Color.FromArgb("#0b64f4");
    private Color _darkSelectionColor = Color.FromArgb("#3c83f6");
    private Color _lightInnerBorderColor = Color.FromArgb("#f3f4f6");
    private Color _darkInnerBorderColor = Color.FromArgb("#1d283a");

    public ModifyTeamPopup(Team team, Member member, HomeViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _teamToEdit = team;
        _memberToEdit = member;

        _selectedIconSource = !string.IsNullOrEmpty(team.Icon) ? team.Icon : "cat.png";
        _selectedTeamColor = team.TeamColor;
        TeamNameEntry.Text = team.Name;

        this.Opened += OnPopupOpened;
    }

    private void OnPopupOpened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        foreach (var child in IconContainer.Children)
        {
            if (child is Border outerBorder &&
                outerBorder.Content is Border innerBorder &&
                innerBorder.Content is Image img)
            {
                if (img.Source.ToString().Contains(_selectedIconSource))
                {
                    SelectIcon(outerBorder);
                    break;
                }
            }
        }

        foreach (var child in ColorContainer.Children)
        {
            if (child is Border outerBorder &&
                outerBorder.Content is Border innerBorder)
            {
                if (ColorsAreClose(innerBorder.BackgroundColor, _selectedTeamColor))
                {
                    SelectColor(outerBorder);
                    break;
                }
            }
        }
    }

    private bool ColorsAreClose(Color a, Color b)
    {
        return Math.Abs(a.Red - b.Red) < 0.01 &&
               Math.Abs(a.Green - b.Green) < 0.01 &&
               Math.Abs(a.Blue - b.Blue) < 0.01;
    }

    private void OnModifyClicked(object sender, EventArgs e)
    {
        string teamName = TeamNameEntry.Text;
        if (string.IsNullOrWhiteSpace(teamName))
        {
            TeamNameEntry.PlaceholderColor = Colors.Red;
            return;
        }

        if (_viewModel != null && _teamToEdit != null && _memberToEdit != null)
        {

            _teamToEdit.Name = teamName;
            _teamToEdit.Icon = _selectedIconSource;
            _teamToEdit.TeamColor = _selectedTeamColor;

            if (_memberToEdit != null)
            {
                _memberToEdit.BaseColor = _selectedTeamColor;
            }
        }

        Close();
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }

    private void OnIconTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border tappedBorder && e.Parameter is string iconName)
        {
            _selectedIconSource = iconName;
            SelectIcon(tappedBorder);
        }
    }

    private void OnColorTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border tappedBorder && tappedBorder.Content is Border innerBorder)
        {
            if (innerBorder.Content is Grid grid && grid.Children[0] is BoxView box)
            {
                _selectedTeamColor = innerBorder.BackgroundColor;
            }
            else
            {
                _selectedTeamColor = innerBorder.BackgroundColor;
            }

            SelectColor(tappedBorder);
        }
    }

    private void SelectIcon(Border border)
    {
        bool isDark = Application.Current.RequestedTheme == AppTheme.Dark;
        var selectionColor = isDark ? _darkSelectionColor : _lightSelectionColor;
        var defaultInnerColor = isDark ? _darkInnerBorderColor : _lightInnerBorderColor;

        if (_selectedIconBorder != null)
        {
            _selectedIconBorder.Stroke = Colors.Transparent;
            if (_selectedIconBorder.Content is Border prevInner)
            {
                prevInner.Stroke = defaultInnerColor; 
                prevInner.BackgroundColor = defaultInnerColor;
            }
        }

        border.Stroke = selectionColor;
        if (border.Content is Border currentInner)
        {
            currentInner.Stroke = selectionColor;
            currentInner.BackgroundColor = selectionColor;
        }

        _selectedIconBorder = border;
    }

    private void SelectColor(Border border)
    {
        bool isDark = Application.Current.RequestedTheme == AppTheme.Dark;
        var selectionColor = isDark ? _darkSelectionColor : _lightSelectionColor;

        if (_selectedColorBorder != null)
        {
            _selectedColorBorder.Stroke = Colors.Transparent;
        }

        border.Stroke = selectionColor;
        _selectedColorBorder = border;
    }
}