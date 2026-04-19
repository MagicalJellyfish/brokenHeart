using brokenHeart.Database.DAO;
using brokenHeart.DB;

namespace brokenHeart.Services.DataTransfer.Save.Entities
{
    internal class UserSimplifiedSaveService : IUserSimplifiedSaveService
    {
        private readonly BrokenDbContext _context;

        public UserSimplifiedSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public string UpdateDefaultCharacterAndReturnName(ulong discordId, int charId)
        {
            UserSimplified user = _context.UserSimplified.Single(x => x.DiscordId == discordId);

            var character = _context
                .Characters.Select(x => new { x.Id, x.Name })
                .Single(x => x.Id == user.ActiveCharacterId);

            user.ActiveCharacterId = character.Id;
            _context.SaveChanges();

            return character.Name;
        }

        public void UpdateDefaultAbility(ulong discordId, string shortcut)
        {
            UserSimplified user = _context.UserSimplified.Single(x => x.DiscordId == discordId);
            user.DefaultAbilityString = shortcut;
            _context.SaveChanges();
        }

        public void UpdateDefaultTarget(ulong discordId, string targets)
        {
            UserSimplified user = _context.UserSimplified.Single(x => x.DiscordId == discordId);
            user.DefaultTargetString = targets;
            _context.SaveChanges();
        }
    }
}
