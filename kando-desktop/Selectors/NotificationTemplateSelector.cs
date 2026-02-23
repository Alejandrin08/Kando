using kando_desktop.Models;
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

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var notification = item as NotificationItem;
            if (notification == null) return null;

            return notification.NotificationType == "InviteTeam" ? InviteTemplate : TaskTemplate;
        }
    }
}
