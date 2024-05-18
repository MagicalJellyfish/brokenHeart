using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace brokenHeart.Controllers
{
    [Authorize]
    public class SignalRHub : Hub
    {
        public async Task RegisterForCharChange(int id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"charChanged/{id}");
        }

        public async Task UnregisterFromCharChange(int id)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"charChanged/{id}");
        }
    }
}
