namespace kando_desktop.DTOs.Requests
{
    public class ValidateCodeDto
    {
        public string Email { get; set; } = null!;

        public string Code { get; set; } = null!;
    }
}