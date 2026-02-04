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
    }
}
