using brokenHeart.Database.DAO.Stats;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection.Stats
{
    internal class StatProjectionService : IStatProjectionService
    {
        private readonly BrokenDbContext _context;

        public StatProjectionService(BrokenDbContext context)
        {
            _context = context;
        }

        public List<StatModel> GetStats(int? id = null)
        {
            IQueryable<Stat> stats = _context.Stats;

            if (id != null)
                stats = stats.Where(x => x.Id == id);

            return stats.Select(x => new StatModel() { Id = x.Id, Name = x.Name }).ToList();
        }
    }
}
