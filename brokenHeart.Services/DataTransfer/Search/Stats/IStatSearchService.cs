using brokenHeart.Database.DAO.Stats;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.Stats
{
    internal interface IStatSearchService
    {
        public IQueryable<Stat> GetStats(StatSearch search);
    }
}
