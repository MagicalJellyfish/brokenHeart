using brokenHeart.Database.Utility;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace brokenHeart.Services.SignalR
{
    public class SignalRMessagingService : IHostedService
    {
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly DatabaseEventEmitter _dbEventEmitter;

        public SignalRMessagingService(
            DatabaseEventEmitter dbEventEmitter,
            IHubContext<SignalRHub> hubContext
        )
        {
            _hubContext = hubContext;
            _dbEventEmitter = dbEventEmitter;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dbEventEmitter.CharacterChanged += SendCharacterUpdate;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _dbEventEmitter.CharacterChanged -= SendCharacterUpdate;
            return Task.CompletedTask;
        }

        public void SendCharacterUpdate(object? sender, int changedChar)
        {
            _hubContext
                .Clients.Group($"charChanged/{changedChar}")
                .SendAsync($"charChanged/{changedChar}");
        }
    }
}
