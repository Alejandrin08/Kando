namespace kando_desktop.DTOs.Requests
{
    public class UpdatePasswordDto
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}