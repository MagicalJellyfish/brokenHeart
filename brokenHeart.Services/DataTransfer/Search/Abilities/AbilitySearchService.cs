using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal class AbilitySearchService : SearchService, IAbilitySearchService
    {
        public AbilitySearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<Ability> GetAbilities(AbilitySearch search)
        {
            IQueryable<Ability> abilities = _context.Abilities;

            if (search.Id != null)
            {
                abilities = abilities.Where(x => x.Id == search.Id);
            }

            return abilities;
        }
    }
}
