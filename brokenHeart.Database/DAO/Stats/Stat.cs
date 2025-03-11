using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Stats
{
    public class Stat : IDao
    {
        [JsonConstructor]
        public Stat() { }

        public Stat(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StatValue> StatValues { get; set; } = new List<StatValue>();
    }
}
