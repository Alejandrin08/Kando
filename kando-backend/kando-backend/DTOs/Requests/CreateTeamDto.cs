using System.ComponentModel.DataAnnotations;

namespace kando_backend.DTOs.Requests
{
    public class CreateTeamDto
    {
        [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El ícono del equipo es obligatorio.")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "El color del equipo es obligatorio.")]
        [StringLength(50)]
        public string Color { get; set; }
    }
}
