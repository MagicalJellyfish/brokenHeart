using brokenHeart.DB;
using Microsoft.AspNetCore.SignalR;

namespace brokenHeart.Services.SignalR
{
    public class SignalRMessagingService
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        public SignalRMessagingService(BrokenDbContext context, IHubContext<SignalRHub> hubContext)
        {
            _hubContext = hubContext;

            context.CharacterChanged += SendCharacterUpdate;
        }

        public void SendCharacterUpdate(object? sender, int changedChar)
        {
            _hubContext
                .Clients.Group($"charChanged/{changedChar}")
                .SendAsync($"charChanged/{changedChar}");
        }
    }
}
