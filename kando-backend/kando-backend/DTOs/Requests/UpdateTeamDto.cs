using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class UpdateTeamDto
    {
        [Required(ErrorMessage = "Team Name is required.")]
        [StringLength(255, ErrorMessage = "The name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The team icon is required.")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "The team color is required.")]
        [StringLength(50)]
        public string Color { get; set; }
    }
}
