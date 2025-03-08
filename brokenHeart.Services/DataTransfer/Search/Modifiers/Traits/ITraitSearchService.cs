using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.Models.DataTransfer.Search.Modifiers;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal interface ITraitSearchService
    {
        public IQueryable<Trait> GetTraits(TraitSearch search);
    }
}
