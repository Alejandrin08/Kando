#if WINDOWS
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml.Controls;

namespace kando_desktop.Platforms.Windows
{
    public class CustomShellHandler : ShellHandler
    {
        protected override void ConnectHandler(ShellView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.Loaded += (s, e) =>
            {
                HideNavigationButtons(platformView);
            };
        }

        private void HideNavigationButtons(Microsoft.UI.Xaml.DependencyObject parent)
        {
            var navView = FindNavView(parent);
            if (navView != null)
            {
                navView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
                navView.IsBackEnabled = false;
                navView.IsPaneToggleButtonVisible = false;
                navView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
                navView.IsPaneOpen = false;
                navView.OpenPaneLength = 0;
            }

            var toggleBtn = FindElementByName(parent, "TogglePaneButton");
            if (toggleBtn != null)
                toggleBtn.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;

            var backBtn = FindElementByName(parent, "NavigationViewBackButton");
            if (backBtn != null)
                backBtn.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        private NavigationView? FindNavView(Microsoft.UI.Xaml.DependencyObject parent)
        {
            var count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is NavigationView nv)
                    return nv;

                var result = FindNavView(child);
                if (result != null) return result;
            }
            return null;
        }

        private Microsoft.UI.Xaml.FrameworkElement? FindElementByName(
            Microsoft.UI.Xaml.DependencyObject parent, string name)
        {
            var count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is Microsoft.UI.Xaml.FrameworkElement fe && fe.Name == name)
                    return fe;

                var result = FindElementByName(child, name);
                if (result != null) return result;
            }
            return null;
        }
    }
}
#endif