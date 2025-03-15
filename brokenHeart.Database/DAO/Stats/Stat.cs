using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Stats
{
    public class Stat : IDao
    {
        [JsonConstructor]
        public Stat() { }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<StatValue> StatValues { get; set; } = new List<StatValue>();
    }
}
