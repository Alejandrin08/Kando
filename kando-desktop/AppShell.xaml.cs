namespace kando_desktop
{
    public partial class AppShell : Shell
    {
        public AppShell(string targetRoute = null)
        {
            InitializeComponent();

            Routing.RegisterRoute("RegisterPage", typeof(Views.ContentPages.RegisterPage));
            Routing.RegisterRoute("LoginPage", typeof(Views.ContentPages.LoginPage)); 
            Routing.RegisterRoute("HomePage", typeof(Views.ContentPages.HomePage));

            if (!string.IsNullOrEmpty(targetRoute))
            {
                if (!targetRoute.StartsWith("//"))
                {
                    targetRoute = $"//{targetRoute}";
                }

                Dispatcher.Dispatch(async () =>
                {
                    try
                    {
                        await GoToAsync(targetRoute);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error navegando: {ex.Message}");
                        await GoToAsync("//LoginPage");
                    }
                });
            }
        }
    }
}