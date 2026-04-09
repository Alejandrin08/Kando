using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace kando_backend.DTOs.Requests
{
    public class InviteMemberRequestDto
    {
        [Required(ErrorMessage = "ID is required")]
        [Key]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string EmailToInvite { get; set; }
    }
}
