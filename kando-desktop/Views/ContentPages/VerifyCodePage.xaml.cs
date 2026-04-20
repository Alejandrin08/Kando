using kando_desktop.Services.Contracts;
using kando_desktop.ViewModels.ContentPages;

namespace kando_desktop.Views.ContentPages
{
    public partial class VerifyCodePage : ContentPage
    {
        private readonly INotificationService _notificationService;
        private bool _isHandlingKey = false;
        private const string ZeroWidthSpace = "\u200B";
        private List<Entry> _entries;

        public VerifyCodePage(VerifyCodeViewModel verifyCodeViewModel, INotificationService notificationService)
        {
            InitializeComponent();
            BindingContext = verifyCodeViewModel;
            _notificationService = notificationService;

            _entries = new List<Entry>
            {
                CodeEntry1, CodeEntry2, CodeEntry3,
                CodeEntry4, CodeEntry5, CodeEntry6
            };

            foreach (var entry in _entries)
                entry.Text = ZeroWidthSpace;

#if WINDOWS
            foreach (var entry in _entries)
            {
                var capturedEntry = entry;
                capturedEntry.HandlerChanged += (s, e) =>
                {
                    if (capturedEntry.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
                    {
                        kando_desktop.Platforms.Windows.CodeEntryKeyHandler.Attach(
                            textBox,
                            onBackspace: () => MainThread.BeginInvokeOnMainThread(() => OnBackspacePressed(capturedEntry)),
                            onDigit: (digit) => MainThread.BeginInvokeOnMainThread(() => OnDigitPressed(capturedEntry, digit))
                        );
                    }
                };
            }
#endif

            _notificationService.OnShowNotification += async (msg, isError) =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await NotificationToast.ShowToast(msg, isError);
                });
            };
        }

        private void OnDigitPressed(Entry entry, string digit)
        {
            if (_isHandlingKey) return;
            _isHandlingKey = true;

            entry.Text = digit;

            var next = GetNextEntry(entry);
            if (next != null)
            {
                next.Text = ZeroWidthSpace;
                next.Focus();
            }

            _isHandlingKey = false;
            UpdateViewModelCode();
        }

        private void OnBackspacePressed(Entry entry)
        {
            if (_isHandlingKey) return;
            _isHandlingKey = true;

            string cleanText = (entry.Text ?? string.Empty).Replace(ZeroWidthSpace, "");

            if (cleanText.Length > 0)
            {
                entry.Text = ZeroWidthSpace;
            }
            else
            {
                var previous = GetPreviousEntry(entry);
                if (previous != null)
                {
                    _isHandlingKey = false;
                    previous.Focus();
                    _isHandlingKey = true;
                    previous.Text = ZeroWidthSpace;
                }
            }

            _isHandlingKey = false;
            UpdateViewModelCode();
        }

        private void OnCodeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isHandlingKey) return;

            var entry = sender as Entry;
            if (entry == null) return;

            string newText = e.NewTextValue ?? string.Empty;
            string clean = newText.Replace(ZeroWidthSpace, "");

            if (clean.Length > 0 && !char.IsDigit(clean[0]))
            {
                _isHandlingKey = true;
                entry.Text = e.OldTextValue ?? ZeroWidthSpace;
                _isHandlingKey = false;
            }
        }

        private void OnResendPointerEntered(object sender, PointerEventArgs e)
        {
            if (BindingContext is VerifyCodeViewModel vm && !vm.CanResend)
                return;

            if (sender is Label label) label.TextDecorations = TextDecorations.Underline;
        }

        private void OnResendPointerExited(object sender, PointerEventArgs e)
        {
            if (sender is Label label) label.TextDecorations = TextDecorations.None;
        }

        private Entry? GetPreviousEntry(Entry current)
        {
            int index = _entries.IndexOf(current);
            return index > 0 ? _entries[index - 1] : null;
        }

        private Entry? GetNextEntry(Entry current)
        {
            int index = _entries.IndexOf(current);
            return index < _entries.Count - 1 ? _entries[index + 1] : null;
        }

        private void UpdateViewModelCode()
        {
            if (BindingContext is VerifyCodeViewModel vm)
            {
                string finalCode = string.Concat(
                    _entries.Select(e => e.Text?.Replace(ZeroWidthSpace, "") ?? ""));
                vm.VerificationCode = finalCode;
            }
        }
    }
}