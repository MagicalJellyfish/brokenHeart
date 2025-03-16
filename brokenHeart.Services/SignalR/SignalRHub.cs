using brokenHeart.Authentication.DB;
using brokenHeart.Database.DAO;
using brokenHeart.DB;
using brokenHeart.Models.Rolling;
using brokenHeart.Services.Rolling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.SignalR
{
    [Authorize]
    public class SignalRHub : Hub
    {
        private readonly BrokenDbContext _context;
        private readonly AuthDbContext _authContext;
        private readonly IRollService _rollService;

        public SignalRHub(
            BrokenDbContext context,
            AuthDbContext authContext,
            IRollService rollService
        )
            : base()
        {
            _context = context;
            _authContext = authContext;
            _rollService = rollService;
        }

        public override Task OnConnectedAsync()
        {
            if (Context.UserIdentifier == "localhost")
            {
                Groups.AddToGroupAsync(Context.ConnectionId, "brokenHand");
            }

            return base.OnConnectedAsync();
        }

        public async Task RollStat(int charId, int statId)
        {
            string? requestUsername = Context.GetHttpContext().User.Identity?.Name?.ToLower();

            if (requestUsername == null)
            {
                return;
            }

            ulong? discordId = _authContext
                .Users.Where(x => x.Username.ToLower() == requestUsername.ToLower())
                .SingleOrDefault()
                ?.DiscordId;

            if (discordId == null)
            {
                return;
            }

            Character? c = GetBaseCharacters().SingleOrDefault(x => x.Id == charId);
            if (c == null)
            {
                await Clients
                    .Client(Context.ConnectionId)
                    .SendAsync("error", $"No character found for ID {charId}");
                return;
            }

            string statName = _context.Stats.SingleOrDefault(x => x.Id == statId).Name[..3];
            if (statName == null)
            {
                await Clients
                    .Client(Context.ConnectionId)
                    .SendAsync("error", $"No stat found for ID {statId}");
                return;
            }

            string roll = $"1d20+[{statName.ToUpper()}]";
            RollResult result = _rollService.CharRollString(roll, c);

            await Clients.Group("brokenHand").SendAsync("web/Roll", roll, result, discordId);
        }

        public async Task RegisterForCharChange(int id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"charChanged/{id}");
        }

        public async Task UnregisterFromCharChange(int id)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"charChanged/{id}");
        }

        // csharpier-ignore
        private IQueryable<Character> GetBaseCharacters()
        {
            return _context.Characters
                .Include(x => x.Counters)

                .Include(x => x.Items).ThenInclude(x => x.Counters)

                .Include(x => x.Effects).ThenInclude(x => x.Counters)
                .Include(x => x.Effects).ThenInclude(x => x.EffectCounter)

                .Include(x => x.Traits).ThenInclude(x => x.Counters)

                .Include(x => x.Stats).ThenInclude(x => x.Stat);
        }
    }
}
