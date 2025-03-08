using brokenHeart.Database.DAO.Counters;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal interface ICounterSearchService
    {
        public IQueryable<Counter> GetCounters(CounterSearch search);
    }
}
