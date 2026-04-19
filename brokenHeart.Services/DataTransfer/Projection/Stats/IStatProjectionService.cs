using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection.Stats
{
    public interface IStatProjectionService
    {
        public List<StatModel> GetStats(int? id = null);
    }
}
