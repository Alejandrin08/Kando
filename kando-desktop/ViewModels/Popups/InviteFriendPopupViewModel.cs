using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.ViewModels.Popups
{
    public class InviteFriendPopupViewModel
    {

        private readonly Team _team;
        private readonly ITeamService _teamService;
        private readonly INotificationService _notificationService;

        public Action RequestClose;

        public InviteFriendPopupViewModel(Team team, ITeamService teamService, INotificationService notificationService)
        {
            _team = team;
            _teamService = teamService;
            _notificationService = notificationService;
        }


    }
}
