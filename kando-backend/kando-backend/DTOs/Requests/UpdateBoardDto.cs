using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class UpdateBoardDto
    {
        [Required(ErrorMessage = "The name of the board is required.")]
        [StringLength(255, ErrorMessage = "The name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The board icon is required.")]
        public string Icon { get; set; }
    }
}
