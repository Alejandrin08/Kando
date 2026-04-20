namespace kando_desktop.DTOs.Requests
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string NewPassword { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;
    }
}