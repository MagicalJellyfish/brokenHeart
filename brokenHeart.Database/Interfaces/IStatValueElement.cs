using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Database.Interfaces
{
    public interface IStatValueElement : IDao
    {
        public ICollection<StatValue> StatIncreases { get; set; }
    }
}
