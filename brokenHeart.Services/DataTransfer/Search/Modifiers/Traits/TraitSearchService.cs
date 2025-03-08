using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search.Modifiers;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal class TraitSearchService : SearchService, ITraitSearchService
    {
        public TraitSearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<Trait> GetTraits(TraitSearch search)
        {
            IQueryable<Trait> traits = _context.Traits;

            if (search.Id != null)
            {
                traits = traits.Where(x => x.Id == search.Id);
            }

            return traits;
        }
    }
}
