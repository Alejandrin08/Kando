using kando_backend.DTOs.Requests;
using kando_backend.DTOs.Responses;
using kando_backend.Models;
using kando_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace kando_backend.Services.Implementations
{
    public class TeamService : ITeamService
    {

        private readonly KandoDbContext _context;

        public TeamService(KandoDbContext context)
        {
            _context = context;
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
    }
}
