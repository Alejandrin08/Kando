namespace kando_backend.DTOs.Requests
{
    public class InviteMemberRequestDto
    {
        public int TeamId { get; set; }
        public string EmailToInvite { get; set; }
    }
}
