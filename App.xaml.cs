namespace kando_desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            window.MinimumWidth = 800;
            window.MinimumHeight = 600;

            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

            var screenWidth = displayInfo.Width / displayInfo.Density;
            var screenHeight = displayInfo.Height / displayInfo.Density;

            window.X = (screenWidth - window.Width) / 2;
            window.Y = (screenHeight - window.Height) / 2;

            return window;
        }
    }
}