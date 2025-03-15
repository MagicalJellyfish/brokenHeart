using brokenHeart.Database.DAO.Abilities;

namespace brokenHeart.Database.Interfaces
{
    public interface IRolling : IDao
    {
        public ICollection<Roll> Rolls { get; set; }
    }
}
