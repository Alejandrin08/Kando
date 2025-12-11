using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace kando_desktop.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EyeIconSource))]
        private bool isPasswordHidden = true;

        public string EyeIconSource => IsPasswordHidden ? "show.png" : "hide.png";

        [RelayCommand]
        private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

        [RelayCommand]
        private async Task GoToLogin()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}