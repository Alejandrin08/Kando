using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class UpdateInvitationDecisionDto
    {
        [Required]
        [RegularExpression("Active|Rejected", ErrorMessage = "Status must be Active or Rejected")]
        public string Status { get; set; }
    }
}