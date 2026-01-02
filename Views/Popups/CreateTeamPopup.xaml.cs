namespace kando_desktop.Views.Popups;

using CommunityToolkit.Maui.Views;
using kando_desktop.ViewModels;
using Microsoft.Maui.Controls;

public partial class CreateTeamPopup : Popup
{
    private HomeViewModel _viewModel;

    private string _selectedIconSource = "cat.png";
    private Color _selectedTeamColor = Color.FromArgb("#8f45ef");

    private Border _selectedIconBorder;
    private Border _selectedColorBorder;

    private Color _lightSelectionColor = Color.FromArgb("#0b64f4");
    private Color _darkSelectionColor = Color.FromArgb("#3c83f6");
    private Color _lightInnerBorderColor = Color.FromArgb("#f3f4f6");
    private Color _darkInnerBorderColor = Color.FromArgb("#1d283a");

    public CreateTeamPopup(BaseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        this.Opened += OnPopupOpened;
    }

    private void OnCreateClicked(object sender, EventArgs e)
    {
        string teamName = TeamNameEntry.Text;
        if (string.IsNullOrWhiteSpace(teamName))
        {
            TeamNameEntry.PlaceholderColor = Colors.Red; 
            return;
        }

        var vm = _viewModel ?? BindingContext as HomeViewModel;

        if (vm != null)
        {
            vm.AddNewTeam(teamName, _selectedIconSource, _selectedTeamColor);
        }

        Close();
    }

    private void OnPopupOpened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        if (IconContainer.Children.Count > 0 && IconContainer.Children[0] is Border firstIcon)
        {
            SelectIcon(firstIcon);
        }

        if (ColorContainer.Children.Count > 0 && ColorContainer.Children[0] is Border firstColor)
        {
            SelectColor(firstColor);
        }
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }

    private void OnIconTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border tappedBorder)
        {
            SelectIcon(tappedBorder);

            if (e.Parameter is string iconName)
            {
                _selectedIconSource = iconName.EndsWith(".png") ? iconName : $"{iconName}.png";
            }
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

            if (_selectedIconBorder.Content is Border previousInnerBorder)
            {
                previousInnerBorder.Stroke = defaultInnerColor;
                previousInnerBorder.BackgroundColor = defaultInnerColor;
            }
        }

        border.Stroke = selectionColor;

        if (border.Content is Border currentInnerBorder)
        {
            currentInnerBorder.Stroke = selectionColor;
            currentInnerBorder.BackgroundColor = selectionColor;
        }

        _selectedIconBorder = border;
    }

    private void OnColorTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border tappedBorder)
        {
            SelectColor(tappedBorder);

            if (tappedBorder.Content is Border innerBorder)
            {
                _selectedTeamColor = innerBorder.BackgroundColor;
            }
        }
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