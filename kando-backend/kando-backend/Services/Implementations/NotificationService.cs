using kando_backend.DTOs.Responses;
using kando_backend.Hubs;
using kando_backend.Models;
using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace kando_backend.Services.Implementations
{
    public class NotificationService : INotificationService
    {

        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly KandoDbContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, KandoDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task SendNotificationToUserAsync(int userId, object notificationPayload)
        {
            await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", notificationPayload);
        }

        public async Task<List<NotificationResponseDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.ToUserId == userId)
                .Where(n => n.Type != "InviteTeam" ||
                            _context.TeamMembers.Any(tm => tm.TeamId == n.TeamId
                                                        && tm.UserId == userId
                                                        && tm.Status == "Pending"))
                .Where(n => n.Type != "TaskAssigned" ||
                            (n.Task != null && n.Task.IsDeleted != true))
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    Id = n.Id,
                    NotificationType = n.Type,
                    IsRead = n.IsRead ?? false,
                    CreatedAt = n.CreatedAt ?? DateTime.UtcNow,

                    OwnerName = n.FromUser != null ? n.FromUser.Username : null,

                    TeamId = n.TeamId,
                    TeamName = n.Team != null ? n.Team.Name : null,
                    TeamIcon = n.Team != null ? n.Team.Icon : null,
                    TeamColor = n.Team != null ? n.Team.Color : null,

                    TaskId = n.TaskId,
                    TaskName = n.Task != null ? n.Task.Title : null,

                    BoardName = n.Task != null && n.Task.List != null && n.Task.List.Board != null
                                ? n.Task.List.Board.Name
                                : null
                })
                .ToListAsync();

            return notifications;
        }
    }
}