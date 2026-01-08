using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Behaviors
{
    public class SelectionBehavior : Behavior<Border>
    {
        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(
                nameof(IsSelected),
                typeof(bool),
                typeof(SelectionBehavior),
                false,
                propertyChanged: OnIsSelectedChanged);

        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create(
                nameof(SelectedColor),
                typeof(Color),
                typeof(SelectionBehavior),
                Colors.Blue);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        private Border _border;

        protected override void OnAttachedTo(Border border)
        {
            base.OnAttachedTo(border);
            _border = border;
            UpdateSelection(IsSelected);
        }

        protected override void OnDetachingFrom(Border border)
        {
            base.OnDetachingFrom(border);
            _border = null;
        }

        private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SelectionBehavior behavior)
            {
                behavior.UpdateSelection((bool)newValue);
            }
        }

        private void UpdateSelection(bool isSelected)
        {
            if (_border == null) return;

            _border.Stroke = isSelected ? SelectedColor : Colors.Transparent;
            _border.StrokeThickness = isSelected ? 2 : 0;
        }
    }
}
