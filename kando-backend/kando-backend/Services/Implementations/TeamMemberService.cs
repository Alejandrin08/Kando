using kando_backend.DTOs.Responses;
using kando_backend.Hubs;
using kando_backend.Models;
using kando_backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace kando_backend.Services.Implementations
{
    public class TeamMemberService : ITeamMemberService
    {

        private readonly KandoDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TeamMemberService(KandoDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<bool> RemoveMemberAsync(int teamId, int userIdToRemove, int ownerId)
        {
            if (userIdToRemove == ownerId) return false;

            var teamMember = await _context.TeamMembers
                .Include(t => t.Team)
                .FirstOrDefaultAsync(t =>
                    t.TeamId == teamId &&
                    t.UserId == userIdToRemove &&
                    (t.Status == "Active" || t.Status == "Pending") &&
                    t.Team.OwnerId == ownerId);

            if (teamMember == null) return false;

            string previousStatus = teamMember.Status;

            teamMember.Status = "Removed";
            teamMember.RemovedAt = DateTime.UtcNow;

            var ownerUser = await _context.Users.FindAsync(ownerId);
            string teamName = teamMember.Team.Name;
            string ownerName = ownerUser?.Username ?? "Owner";

            if (previousStatus == "Pending")
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.TeamId == teamId && n.ToUserId == userIdToRemove && n.Type == "InviteTeam");
                if (notification != null)
                {
                    _context.Notifications.Remove(notification);
                }
            }

            await _context.SaveChangesAsync();

            if (previousStatus == "Pending")
            {
                await _hubContext.Clients.Group(userIdToRemove.ToString()).SendAsync("InvitationRevoked", teamId);
            }
            else
            {
                await _hubContext.Clients.Group(userIdToRemove.ToString()).SendAsync("RemovedFromTeam", teamId, teamName, ownerName);
            }

            await _hubContext.Clients.Group($"Team_{teamId}").SendAsync("TeamMembersChanged", teamId);
            return true;
        }

        public async Task<List<TeamMemberDto>> GetTeamMembersAsync(int teamId, int ownerId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId && t.OwnerId == ownerId);
            if (team == null) return new List<TeamMemberDto>();

            var members = await _context.TeamMembers
                .Include(t => t.User)
                .Where(t => t.TeamId == teamId &&
                            t.UserId != ownerId &&
                            (t.Status == "Active" || t.Status == "Pending"))
                .Select(t => new TeamMemberDto
                {
                    UserId = t.UserId,
                    Name = t.User.Username,
                    Role = t.Role,
                    Status = t.Status
                })
                .ToListAsync();

            return members;
        }
    }
}