using brokenHeart.Database.DAO.Stats;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.Stats
{
    internal class StatSearchService : SearchService, IStatSearchService
    {
        public StatSearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<Stat> GetStats(StatSearch search)
        {
            IQueryable<Stat> stats = _context.Stats;

            if (search.Id != null)
            {
                stats = stats.Where(x => x.Id == search.Id);
            }

            return stats;
        }
    }
}
