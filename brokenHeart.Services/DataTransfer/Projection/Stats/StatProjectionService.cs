using brokenHeart.Database.DAO.Stats;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search.Stats;

namespace brokenHeart.Services.DataTransfer.Projection.Stats
{
    internal class StatProjectionService : IStatProjectionService
    {
        private readonly IStatSearchService _statSearchService;

        public StatProjectionService(IStatSearchService statSearchService)
        {
            _statSearchService = statSearchService;
        }

        public List<StatModel> GetStats(StatSearch search)
        {
            IQueryable<Stat> stats = _statSearchService.GetStats(search);

            return stats.Select(x => new StatModel() { Id = x.Id, Name = x.Name, }).ToList();
        }
    }
}
