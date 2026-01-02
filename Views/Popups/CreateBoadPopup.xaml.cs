namespace kando_desktop.Views.Popups;

using CommunityToolkit.Maui.Views;
using kando_desktop.ViewModels;
using Microsoft.Maui.Controls;

public partial class CreateBoadPopup : Popup
{
    private Border _selectedIconBorder;

    private Color _lightSelectionColor = Color.FromArgb("#0b64f4");
    private Color _darkSelectionColor = Color.FromArgb("#3c83f6");

    private Color _lightInnerBorderColor = Color.FromArgb("#f3f4f6");
    private Color _darkInnerBorderColor = Color.FromArgb("#1d283a");

    public CreateBoadPopup(BaseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.Opened += OnPopupOpened;
    }

    private void OnPopupOpened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        if (IconContainer.Children.Count > 0 && IconContainer.Children[0] is Border firstIcon)
        {
            SelectIcon(firstIcon);
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
}