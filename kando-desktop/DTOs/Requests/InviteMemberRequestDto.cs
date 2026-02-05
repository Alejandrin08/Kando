using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.DTOs.Requests
{
    public class InviteMemberRequestDto
    {
        public int TeamId { get; set; }
        public string EmailToInvite { get; set; }
    }
}
