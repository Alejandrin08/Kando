namespace kando_desktop.Views.Components;

public partial class TeamCardView : ContentView
{
	public TeamCardView()
	{
		InitializeComponent();
	}

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        ThreeDotsLabel.Opacity = 1;
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        ThreeDotsLabel.Opacity = 0;
    }
}