using brokenHeart.Database.DAO.Stats;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Stats
{
    internal class StatProjectionService : IStatProjectionService
    {
        private readonly IDaoSearchService _daoSearchService;

        public StatProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public List<StatModel> GetStats(DaoSearch? search)
        {
            IQueryable<Stat> stats = _daoSearchService.GetElements<Stat>(search);

            return stats.Select(x => new StatModel() { Id = x.Id, Name = x.Name, }).ToList();
        }
    }
}
