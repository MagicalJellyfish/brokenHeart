using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search.Modifiers;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal class EffectSearchService : SearchService, IEffectSearchService
    {
        public EffectSearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<Effect> GetEffects(EffectSearch search)
        {
            IQueryable<Effect> effects = _context.Effects;

            if (search.Id != null)
            {
                effects = effects.Where(x => x.Id == search.Id);
            }

            return effects;
        }
    }
}
