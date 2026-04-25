using kando_desktop.ViewModels.Components;

namespace kando_desktop.Views.Components
{
    public partial class SidebarView : ContentView
    {
        public static readonly BindableProperty TeamIdProperty =
            BindableProperty.Create(nameof(TeamId), typeof(int), typeof(SidebarView), 0, propertyChanged: OnIdsChanged);

        public static readonly BindableProperty CurrentBoardIdProperty =
            BindableProperty.Create(nameof(CurrentBoardId), typeof(int), typeof(SidebarView), 0, propertyChanged: OnIdsChanged);

        public static readonly BindableProperty IsExpandedProperty =
            BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(SidebarView), true);

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public int TeamId
        {
            get => (int)GetValue(TeamIdProperty);
            set => SetValue(TeamIdProperty, value);
        }

        public int CurrentBoardId
        {
            get => (int)GetValue(CurrentBoardIdProperty);
            set => SetValue(CurrentBoardIdProperty, value);
        }

        public SidebarView()
        {
            InitializeComponent();
            Content.BindingContext = new SidebarViewModel();
            WidthRequest = 280;
        }

        private static void OnIdsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (SidebarView)bindable;

            if (view.TeamId != 0 && view.CurrentBoardId != 0)
            {
                if (view.Content?.BindingContext is SidebarViewModel vm)
                {
                    vm.LoadBoards(view.TeamId, view.CurrentBoardId);
                }
            }
        }
    }
}