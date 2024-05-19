using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers
{
    //[Authorize]
    public class SignalRHub : Hub
    {
        private readonly BrokenDbContext _context;

        public SignalRHub(BrokenDbContext context)
            : base()
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated == false)
            {
                Groups.AddToGroupAsync(Context.ConnectionId, "brokenHand");
            }

            return base.OnConnectedAsync();
        }

        public async Task RollStat(int charId, int statId, string discordId)
        {
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
            RollResult result = RollAuxiliary.CharRollString(roll, c);

            await Clients
                .Group("brokenHand")
                .SendAsync("web/Roll", roll, result, ulong.Parse(discordId));
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
