using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Helpers;
using kando_backend.Hubs;
using kando_backend.Models;
using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace kando_backend.Services.Implementations
{
    public class TeamService : ITeamService
    {

        private readonly KandoDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public const int MaxMembersTeam = 5;

        public TeamService(KandoDbContext context, IEmailService emailService, INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        public async Task<TeamResponseDto> CreateTeamAsync(CreateTeamDto createTeamDto, int ownerId)
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

            var ownerMembership = new TeamMember
            {
                UserId = ownerId,
                TeamId = team.Id,
                Role = "Owner",
                Status = "Active",
                JoinedAt = DateTime.UtcNow
            };

            _context.TeamMembers.Add(ownerMembership);
            await _context.SaveChangesAsync();
            var ownerUser = await _context.Users.FindAsync(ownerId);

            return new TeamResponseDto
            {
                Id = team.Id,
                Name = team.Name,
                Icon = team.Icon,
                Color = team.Color,
                CreatedAt = team.CreatedAt.Value,
                IsCurrentUserOwner = true,
                TotalCapacity = 1,
                Members = new List<TeamMemberDto>
                {
                    new TeamMemberDto { UserId = ownerId, Name = ownerUser.Username, Role = "Owner" }
                }
            };
        }

        public async Task<List<TeamResponseDto>> GetTeamsUserAsync(int userId)
        {
            var teams = await _context.Teams
                .Include(t => t.Owner)
                .Include(t => t.TeamMembers)
                    .ThenInclude(tm => tm.User)
                .Where(t => (t.IsDeleted == false || t.IsDeleted == null) &&
                            (t.OwnerId == userId ||
                             t.TeamMembers.Any(tm => tm.UserId == userId && tm.Status == "Active")))
                .AsNoTracking()
                .ToListAsync();

            return teams.Select(t => new TeamResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Icon = t.Icon,
                Color = t.Color,
                CreatedAt = t.CreatedAt.GetValueOrDefault(DateTime.UtcNow),
                Members = BuildMemberList(t),
                IsCurrentUserOwner = t.OwnerId == userId,
                TotalCapacity = t.TeamMembers.Count(tm => tm.Status == "Active" || tm.Status == "Pending")
            }).ToList();
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

            var ownerUser = await _context.Users.FindAsync(ownerId);
            string ownerName = ownerUser?.Username ?? "Owner";

            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group($"Team_{teamId}").SendAsync("TeamUpdated", teamId, updateTeamDto.Name, ownerName, ownerId);
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

            var pendingUserIds = await _context.TeamMembers
                .Where(tm => tm.TeamId == teamId && tm.Status == "Pending")
                .Select(tm => tm.UserId)
                .ToListAsync();

            var teamNotifications = await _context.Notifications
                .Where(n => n.TeamId == teamId && n.Type == "InviteTeam")
                .ToListAsync();

            if (teamNotifications.Any())
            {
                _context.Notifications.RemoveRange(teamNotifications);
            }

            var allTeamMembers = await _context.TeamMembers
                .Where(tm => tm.TeamId == teamId && tm.Status != "Removed")
                .ToListAsync();

            foreach (var tm in allTeamMembers)
            {
                tm.Status = "Removed";
                tm.RemovedAt = now;
            }

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

            var ownerUser = await _context.Users.FindAsync(ownerId);
            string ownerName = ownerUser?.Username ?? "Owner";
            string teamName = existingTeam.Name;

            existingTeam.IsDeleted = true;
            existingTeam.DeletedAt = now;

            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"Team_{teamId}").SendAsync("TeamDeleted", teamId, teamName, ownerName, ownerId);

            foreach (var userId in pendingUserIds)
            {
                await _hubContext.Clients.Group(userId.ToString()).SendAsync("InvitationRevoked", teamId);
            }

            return true;
        }

        public async Task<bool> InviteMemberAsync(int teamId, string emailToInvite, int ownerId)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => t.Id == teamId && t.OwnerId == ownerId && (t.IsDeleted == null || t.IsDeleted == false));

            if (team == null) throw new UnauthorizedAccessException("Team not found, unauthorized or deleted.");

            var currentMemberCount = await _context.TeamMembers
                .CountAsync(tm => tm.TeamId == teamId && (tm.Status == "Active" || tm.Status == "Pending"));

            if (currentMemberCount + 1 > MaxMembersTeam)
                throw new InvalidOperationException("The team has reached its maximum capacity (5 members including pending invites).");

            var userToInvite = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailToInvite);
            if (userToInvite == null) throw new KeyNotFoundException("User not found.");

            if (userToInvite.Id == ownerId) throw new InvalidOperationException("Self invite.");

            var existingMembership = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userToInvite.Id);

            if (existingMembership != null)
            {
                if (existingMembership.Status == "Active") throw new InvalidOperationException("User is already a member.");
                if (existingMembership.Status == "Pending") throw new InvalidOperationException("An invitation is already pending for this user.");

                existingMembership.Status = "Pending";
                existingMembership.Role = "PendingMember";
                existingMembership.JoinedAt = null;
                existingMembership.RemovedAt = null;
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

            var existingNotification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.TeamId == teamId && n.ToUserId == userToInvite.Id && n.Type == "InviteTeam");

            Notification notification;
            if (existingNotification != null)
            {
                notification = existingNotification;
                notification.IsRead = false;
                notification.CreatedAt = DateTime.UtcNow;
                notification.FromUserId = ownerId;
            }
            else
            {
                notification = new Notification
                {
                    FromUserId = ownerId,
                    ToUserId = userToInvite.Id,
                    TeamId = teamId,
                    Type = "InviteTeam",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
            }

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
                Title = "Invitación a equipo",
                Message = $"{ownerName} te ha invitado al equipo {team.Name}"
            };

            await _notificationService.SendNotificationToUserAsync(userToInvite.Id, notificationDto);

            string htmlBody = EmailTemplates.GetInvitationTemplate(team.Name, ownerName);
            string subject = $"Kando: Invitación al equipo {team.Name}";
            _ = _emailService.SendEmailAsync(emailToInvite, subject, htmlBody);
            await _hubContext.Clients.Group($"Team_{teamId}").SendAsync("TeamMembersChanged", teamId);
            return true;
        }

        public async Task<bool> UpdateInvitationAsync(int teamId, int userId, UpdateInvitationDecisionDto dto)
        {
            var membership = await _context.TeamMembers
                  .Include(tm => tm.Team)
                  .Include(tm => tm.User)
                  .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (membership == null) throw new KeyNotFoundException("Membership record not found.");

            if (membership.Status != "Pending")
                throw new InvalidOperationException("This invitation has already been processed.");

            membership.Status = dto.Status;

            if (dto.Status == "Active")
            {
                membership.Role = "Member";
                membership.JoinedAt = DateTime.UtcNow;

                var team = await _context.Teams.FindAsync(teamId);
                if (team != null)
                {
                    await _hubContext.Clients.Group($"Team_{teamId}").SendAsync("TeamMembersChanged", teamId);
                }
            }
            else if (dto.Status == "Rejected")
            {
                membership.RemovedAt = DateTime.UtcNow;
                await _hubContext.Clients.Group($"Team_{teamId}").SendAsync("TeamMembersChanged", teamId);
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.TeamId == teamId && n.ToUserId == userId && n.Type == "InviteTeam" && n.IsRead == false);

            if (notification != null)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private List<TeamMemberDto> BuildMemberList(Team team)
        {
            var members = new List<TeamMemberDto>();

            members.Add(new TeamMemberDto
            {
                UserId = team.OwnerId,
                Name = team.Owner.Username,
                Role = "Owner",
                Status = "Active"
            });

            var activeAndPendingMembers = team.TeamMembers
                   .Where(tm => (tm.Status == "Active" || tm.Status == "Pending") && tm.UserId != team.OwnerId)
                   .Select(tm => new TeamMemberDto
                   {
                       UserId = tm.UserId,
                       Name = tm.User.Username,
                       Role = tm.Role,
                       Status = tm.Status
                   });

            members.AddRange(activeAndPendingMembers);
            return members;
        }
    }
}
