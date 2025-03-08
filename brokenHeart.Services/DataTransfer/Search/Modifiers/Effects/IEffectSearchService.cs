using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Models.DataTransfer.Search.Modifiers;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal interface IEffectSearchService
    {
        public IQueryable<Effect> GetEffects(EffectSearch search);
    }
}
