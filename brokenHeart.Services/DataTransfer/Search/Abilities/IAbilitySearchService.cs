using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal interface IAbilitySearchService
    {
        public IQueryable<Ability> GetAbilities(AbilitySearch search);
    }
}
