using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Resources.Strings;
using System.Globalization;

namespace kando_desktop.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string languageCode;

        public BaseViewModel()
        {
            var currentLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();

            LanguageCode = currentLang == "es" ? "ES" : "EN";

            if (Application.Current.UserAppTheme == AppTheme.Unspecified)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
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
        }

        public string ThemeIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "sun.png" : "moon.png";
        public string LanguageIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "language_light.png" : "language_dark.png";
        public string GoogleIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "google_dark.png" : "google_light.png";
        public string GithubIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "github_dark.png" : "github_light.png";
        public string PlusIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "plus_white.png" : "plus_black.png";
        public string GroupIconSource => Application.Current.UserAppTheme == AppTheme.Dark ? "group_white.png" : "group_black.png";


        public Action<object> RequestMenuOpen;

        [RelayCommand]
        private void ToggleMenu(object anchor)
        {
            RequestMenuOpen?.Invoke(anchor);
        }

        [RelayCommand]
        private async Task Logout()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}