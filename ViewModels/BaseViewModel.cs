using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Resources.Strings;
using kando_desktop.Views.Popups;
using System.Globalization;

namespace kando_desktop.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string languageCode;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string userEmail;

        public Action RequestClosePopup;

        public BaseViewModel()
        {
            var currentLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();

            LanguageCode = currentLang == "es" ? "ES" : "EN";

            if (Application.Current.UserAppTheme == AppTheme.Unspecified)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }

            UserName = "Usuario";
            UserEmail = "usuario@gmail.com";
        }

        [RelayCommand]
        private void ToggleLanguage()
        {
            string newCode = LanguageCode == "ES" ? "en" : "es";

            var culture = new CultureInfo(newCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            AppResources.Culture = culture;

            LanguageCode = newCode.ToUpper();

            string currentRoute = Shell.Current.CurrentState.Location.ToString();
            Application.Current.MainPage = new AppShell(currentRoute);
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            Application.Current.UserAppTheme = Application.Current.UserAppTheme == AppTheme.Dark
                ? AppTheme.Light
                : AppTheme.Dark;

            OnPropertyChanged(nameof(ThemeIconSource));
            OnPropertyChanged(nameof(LanguageIconSource));
            OnPropertyChanged(nameof(GoogleIconSource));
            OnPropertyChanged(nameof(GithubIconSource));
            OnPropertyChanged(nameof(PlusIconSource));
            OnPropertyChanged(nameof(GroupIconSource));
            OnPropertyChanged(nameof(MenuIconSource));
            OnPropertyChanged(nameof(DownIconSource));
            OnPropertyChanged(nameof(ErrorIconSource));
            OnPropertyChanged(nameof(CheckIconSource));
        }

        public string ThemeIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "sun.png" : "moon.png";
        public string LanguageIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "language_light.png" : "language_dark.png";
        public string GoogleIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "google_dark.png" : "google_light.png";
        public string GithubIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "github_dark.png" : "github_light.png";
        public string PlusIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "plus_white.png" : "plus_black.png";
        public string GroupIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "group_white.png" : "group_black.png";
        public string MenuIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "menu_white.png" : "menu_black.png";
        public string DownIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "down_white.png" : "down_black.png";
        public string ErrorIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "error_dark.png" : "error_light.png";
        public string CheckIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "check_dark.png" : "check_light.png";


        public Action<object> RequestMenuOpen;

        [RelayCommand]
        private void ToggleMenu(object anchor)
        {
            var popup = new ProfileMenuPopup(this);
            popup.Anchor = anchor as View;
            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        [RelayCommand]
        private async Task Logout()
        {
            RequestClosePopup?.Invoke();

            await Task.Delay(150);

            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}