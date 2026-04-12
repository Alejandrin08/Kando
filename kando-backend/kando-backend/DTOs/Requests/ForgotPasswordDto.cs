using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}