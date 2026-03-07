namespace kando_backend.DTOs.Responses
{
    public class TeamResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new();
        public bool IsCurrentUserOwner { get; set; }
    }

    public class TeamMemberDto
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
