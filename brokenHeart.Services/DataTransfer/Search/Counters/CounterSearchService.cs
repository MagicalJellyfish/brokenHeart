using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal class CounterSearchService : SearchService, ICounterSearchService
    {
        public CounterSearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<Counter> GetCounters(CounterSearch search)
        {
            IQueryable<Counter> counters = _context.Counters;

            if (search.Id != null)
            {
                counters = counters.Where(x => x.Id == search.Id);
            }

            return counters;
        }
    }
}
