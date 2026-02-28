namespace kando_backend.DTOs.Responses
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string NotificationType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamIcon { get; set; }
        public string TeamColor { get; set; }
        public string OwnerName { get; set; }
        public int? TaskId { get; set; }
        public string TaskName { get; set; }
        public string BoardName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}