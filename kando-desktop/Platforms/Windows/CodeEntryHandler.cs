#if WINDOWS
using Microsoft.UI.Xaml.Input;
using VirtualKey = Windows.System.VirtualKey;

namespace kando_desktop.Platforms.Windows
{
    public class CodeEntryKeyHandler
    {
        public static void Attach(
            Microsoft.UI.Xaml.Controls.TextBox textBox,
            Action onBackspace,
            Action<string> onDigit)
        {
            textBox.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == VirtualKey.Back)
                {
                    e.Handled = true;
                    onBackspace?.Invoke();
                    return;
                }

                string digit = null;

                if (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9)
                    digit = ((int)(e.Key - VirtualKey.Number0)).ToString();
                else if (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9)
                    digit = ((int)(e.Key - VirtualKey.NumberPad0)).ToString();

                if (digit != null)
                {
                    e.Handled = true;
                    onDigit?.Invoke(digit);
                }
            };
        }
    }
}
#endif