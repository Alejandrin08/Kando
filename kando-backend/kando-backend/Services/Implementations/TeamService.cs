using kando_backend.DTOs.Requests;
using kando_backend.Models;
using kando_backend.Services.Interfaces;

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
    }
}
