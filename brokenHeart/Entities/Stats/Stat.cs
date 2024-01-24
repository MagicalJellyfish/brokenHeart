using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Stats
{
    public class Stat
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
