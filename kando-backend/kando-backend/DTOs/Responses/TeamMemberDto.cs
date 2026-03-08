namespace kando_backend.DTOs.Responses
{
    public class TeamMemberDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }
}