using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Stats
{
    public interface IStatProjectionService
    {
        public List<StatModel> GetStats(DaoSearch? search = null);
    }
}
