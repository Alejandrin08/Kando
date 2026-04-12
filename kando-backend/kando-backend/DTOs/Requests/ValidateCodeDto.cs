using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class ValidateCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Code { get; set; } = null!;
    }
}