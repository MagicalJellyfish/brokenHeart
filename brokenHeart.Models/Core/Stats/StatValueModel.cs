using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Models.Core.Stats
{
    public class StatValueModel
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public Stat Stat { get; set; }
    }
}
