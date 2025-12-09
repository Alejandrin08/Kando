using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Resources.Strings;
using System.Globalization;

namespace kando_desktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EyeIconSource))]
        private bool isPasswordHidden = true;
        public string EyeIconSource => IsPasswordHidden ? "show.png" : "hide.png";

        [RelayCommand]
        private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;


        [ObservableProperty]
        private string languageCode;

        public string ThemeIconSource => Application.Current.UserAppTheme == AppTheme.Dark
            ? "sun.png"
            : "moon.png";

        public string LanguageIconSource => Application.Current.UserAppTheme == AppTheme.Dark
            ? "language_light.png"
            : "language_dark.png";

        public string GoogleIconSource => Application.Current.UserAppTheme == AppTheme.Dark
            ? "google_dark.png"
            : "google_light.png";

        public string GithubIconSource => Application.Current.UserAppTheme == AppTheme.Dark
            ? "github_dark.png"
            : "github_light.png";


        public LoginViewModel()
        {
            var currentLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            LanguageCode = currentLang == "es" ? "EN" : "ES";

            if (Application.Current.UserAppTheme == AppTheme.Unspecified)
            {
                Application.Current.UserAppTheme = AppTheme.Dark; 
            }
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            if (Application.Current.UserAppTheme == AppTheme.Dark)
                Application.Current.UserAppTheme = AppTheme.Light;
            else
                Application.Current.UserAppTheme = AppTheme.Dark;

            OnPropertyChanged(nameof(ThemeIconSource));
            OnPropertyChanged(nameof(LanguageIconSource));
            OnPropertyChanged(nameof(GoogleIconSource));
            OnPropertyChanged(nameof(GithubIconSource));
        }

        [RelayCommand]
        private void ToggleLanguage()
        {
            string newCode = LanguageCode == "EN" ? "en" : "es";
            var culture = new CultureInfo(newCode);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            AppResources.Culture = culture;
            LanguageCode = newCode.ToUpper();

            Application.Current.MainPage = new AppShell();
        }
    }
}