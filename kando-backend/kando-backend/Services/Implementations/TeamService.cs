using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Helpers;
using kando_backend.Models;
using kando_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace kando_backend.Services.Implementations
{
    public class TeamService : ITeamService
    {

        private readonly KandoDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public TeamService(KandoDbContext context, IEmailService emailService, INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        public async Task<Team> CreateTeamAsync(CreateTeamDto createTeamDto, int ownerId)
        {
            var team = new Team
            {
                Name = createTeamDto.Name,
                Icon = createTeamDto.Icon,
                Color = createTeamDto.Color,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Teams.Add(team);

            await _context.SaveChangesAsync();

            return team;
        }

        public async Task<List<TeamResponseDto>> GetTeamsUserAsync(int ownerId)
        {
            var team = await _context.Teams
                .Where(t => t.OwnerId == ownerId && (t.IsDeleted == false || t.IsDeleted == null))
                .AsNoTracking()
                .Select(t => new TeamResponseDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Icon = t.Icon,
                    Color = t.Color,
                    CreatedAt = t.CreatedAt.GetValueOrDefault(DateTime.UtcNow)
                }).ToListAsync();

            return team;
        }

        public async Task<bool> UpdateTeamAsync(int teamId, UpdateTeamDto updateTeamDto, int ownerId)
        {
            var existingTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId && t.OwnerId == ownerId && (t.IsDeleted == false || t.IsDeleted == null));
            if (existingTeam == null)
            {
                return false;
            }

            existingTeam.Name = updateTeamDto.Name;
            existingTeam.Icon = updateTeamDto.Icon;
            existingTeam.Color = updateTeamDto.Color;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTeamAsync(int teamId, int ownerId)
        {
            var existingTeam = await _context.Teams
                .Include(t => t.Boards)
                    .ThenInclude(b => b.BoardLists)
                        .ThenInclude(l => l.Tasks)
                .FirstOrDefaultAsync(t => t.Id == teamId && t.OwnerId == ownerId && (t.IsDeleted == false || t.IsDeleted == null));

            if (existingTeam == null) return false;

            var now = DateTime.UtcNow;

            var allLists = existingTeam.Boards
                .SelectMany(b => b.BoardLists)
                .Where(l => l.IsDeleted != true)
                .ToList();

            var allTasks = allLists
                .SelectMany(l => l.Tasks)
                .Where(t => t.IsDeleted != true)
                .ToList();

            foreach (var task in allTasks)
            {
                task.IsDeleted = true;
            }

            foreach (var list in allLists)
            {
                list.IsDeleted = true;
            }

            foreach (var board in existingTeam.Boards.Where(b => b.IsDeleted != true))
            {
                board.IsDeleted = true;
                board.DeletedAt = now;
            }

            existingTeam.IsDeleted = true;
            existingTeam.DeletedAt = now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InviteMemberAsync(int teamId, string emailToInvite, int ownerId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId && t.OwnerId == ownerId);
            if (team == null) throw new UnauthorizedAccessException("Team not found or unauthorized.");

            var userToInvite = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailToInvite);
            if (userToInvite == null) throw new KeyNotFoundException("User not found.");

            if (userToInvite.Id == ownerId) throw new InvalidOperationException("Self invite.");

            var existingMembership = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userToInvite.Id);

            if (existingMembership != null)
            {
                if (existingMembership.Status == "Active") throw new InvalidOperationException("Already active.");
                if (existingMembership.Status == "Pending") throw new InvalidOperationException("Already pending.");

                if (existingMembership.Status == "Rejected" || existingMembership.Status == "Removed")
                {
                    existingMembership.Status = "Pending";
                    existingMembership.JoinedAt = null;
                    existingMembership.RemovedAt = null;
                }
            }
            else
            {
                var newMember = new TeamMember
                {
                    UserId = userToInvite.Id,
                    TeamId = teamId,
                    Role = "PendingMember",
                    Status = "Pending"
                };
                _context.TeamMembers.Add(newMember);
            }

            var notification = new Notification
            {
                FromUserId = ownerId,
                ToUserId = userToInvite.Id,
                TeamId = teamId,
                Type = "InviteTeam",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            var ownerUser = await _context.Users.FindAsync(ownerId);
            string ownerName = ownerUser?.Username ?? "Un usuario";

            var notificationDto = new NotificationResponseDto
            {
                Id = notification.Id,
                NotificationType = "InviteTeam",
                IsRead = false,
                CreatedAt = notification.CreatedAt ?? DateTime.UtcNow,
                TeamId = team.Id,
                TeamName = team.Name,
                TeamIcon = team.Icon,
                TeamColor = team.Color,
                OwnerName = ownerName,
                TaskId = null,
                TaskName = null,
                BoardName = null,
                Title = "Invitación a equipo",
                Message = $"{ownerName} te ha invitado al equipo {team.Name}"
            };

            await _notificationService.SendNotificationToUserAsync(userToInvite.Id, notificationDto);

            string htmlBody = EmailTemplates.GetInvitationTemplate(team.Name, ownerName);
            string subject = $"Kando: Invitación al equipo {team.Name}";

            _ = _emailService.SendEmailAsync(emailToInvite, subject, htmlBody);

            return true;
        }
    }
}
