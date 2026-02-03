using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class CreateBoardDto
    {
        [Required(ErrorMessage = "The board name is required.")]
        [StringLength(255, ErrorMessage = "The name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The board icon is required.")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "The team ID is required.")]
        public int TeamId { get; set; }
        }
}
