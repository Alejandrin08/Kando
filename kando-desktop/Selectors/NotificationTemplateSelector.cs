using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Selectors
{
    public class NotificationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InviteTemplate { get; set; }
        public DataTemplate TaskTemplate { get; set; }
        public DataTemplate BoardTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var notification = item as NotificationItem;
            if (notification == null) return null;

            return notification.NotificationType switch
            {
                "InviteTeam" => InviteTemplate,
                "TaskAssigned" => TaskTemplate,
                "BoardUpdate" => BoardTemplate,
                _ => InviteTemplate
            };
        }
    }
}
