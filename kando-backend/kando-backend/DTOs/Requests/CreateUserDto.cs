using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Name required")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid email format")] 
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Password required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{6,}$", ErrorMessage = "Password invalid")]
        public string Password { get; set; }
    }
}
