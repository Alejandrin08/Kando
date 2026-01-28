namespace kando_backend.DTOs.Responses
{
    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserIcon { get; set; }
        public string Token { get; set; }
    }
}
