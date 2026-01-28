using kando_desktop.Enums;
using Microsoft.Maui.Platform;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Reflection;

namespace kando_desktop
{
    public static class CursorExtensions
    {
        public static void SetCustomCursor(this VisualElement visualElement, CursorIcon cursor, IMauiContext? mauiContext)
        {
            if (visualElement.Handler?.PlatformView is UIElement nativeView)
            {
                ApplyCursorLogic(nativeView, cursor);
            }
            else
            {
                visualElement.HandlerChanged += (s, e) =>
                {
                    if (visualElement.Handler?.PlatformView is UIElement view)
                    {
                        ApplyCursorLogic(view, cursor);
                    }
                };
            }
        }

        private static void ApplyCursorLogic(UIElement view, CursorIcon cursor)
        {
            view.PointerEntered -= ViewOnPointerEntered;
            view.PointerExited -= ViewOnPointerExited;

            view.PointerEntered += ViewOnPointerEntered;
            view.PointerExited += ViewOnPointerExited;

            void ViewOnPointerEntered(object sender, PointerRoutedEventArgs e)
            {
                view.ChangeCursor(cursor);
            }

            void ViewOnPointerExited(object sender, PointerRoutedEventArgs e)
            {
                view.ChangeCursor(CursorIcon.Arrow);
            }
        }

        static void ChangeCursor(this UIElement uiElement, CursorIcon cursorIcon)
        {
            var cursorType = GetCursorType(cursorIcon);
            var cursor = InputSystemCursor.Create(cursorType);

            typeof(UIElement).InvokeMember(
                "ProtectedCursor",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance,
                null,
                uiElement,
                new object[] { cursor });
        }

        static InputSystemCursorShape GetCursorType(CursorIcon cursor)
        {
            return cursor switch
            {
                CursorIcon.Hand => InputSystemCursorShape.Hand,
                CursorIcon.IBeam => InputSystemCursorShape.IBeam,
                CursorIcon.Cross => InputSystemCursorShape.Cross,
                CursorIcon.Arrow => InputSystemCursorShape.Arrow,
                CursorIcon.SizeAll => InputSystemCursorShape.SizeAll,
                CursorIcon.Wait => InputSystemCursorShape.Wait,
                _ => InputSystemCursorShape.Arrow,
            };
        }
    }
}