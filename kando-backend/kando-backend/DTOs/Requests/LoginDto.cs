using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{6,}$", ErrorMessage = "Password invalid")]
        public string Password { get; set; }
    }
}
