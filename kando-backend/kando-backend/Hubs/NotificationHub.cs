using Microsoft.AspNetCore.SignalR;

namespace kando_backend.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task JoinTeamsGroups(List<int> teamIds)
        {
            foreach (var teamId in teamIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Team_{teamId}");
            }
        }
    }
}