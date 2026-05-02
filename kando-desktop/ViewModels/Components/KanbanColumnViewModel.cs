using kando_desktop.Helpers;
using kando_desktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using kando_desktop.Services.Contracts;

namespace kando_desktop.ViewModels.Components
{
    public partial class KanbanColumnViewModel : ObservableObject
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly INotificationService _notificationService;

        public KanbanColumnViewModel()
        {
            _workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            _notificationService = ServiceHelper.GetService<INotificationService>();
        }
    }
}