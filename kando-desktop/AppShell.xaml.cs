namespace kando_desktop
{
    public partial class AppShell : Shell
    {
        public AppShell(string targetRoute = null)
        {
            InitializeComponent();

            Routing.RegisterRoute("RegisterPage", typeof(Views.ContentPages.RegisterPage));
            Routing.RegisterRoute("ModifyProfilePage", typeof(Views.ContentPages.ModifyProfilePage));
            Routing.RegisterRoute("VerifyEmailPage", typeof(Views.ContentPages.VerifyEmailPage));
            Routing.RegisterRoute("VerifyCodePage", typeof(Views.ContentPages.VerifyCodePage));
            Routing.RegisterRoute("ResetPasswordPage", typeof(Views.ContentPages.ResetPasswordPage));

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