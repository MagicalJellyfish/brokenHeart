using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Combat
{
    public class Event : IDao
    {
        [JsonConstructor]
        public Event() { }

        public Event(string name, int round, bool secret, int init = 0)
        {
            Name = name;
            Round = round;
            Secret = secret;
            Init = init;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Round { get; set; }
        public int Init { get; set; }
        public bool Secret { get; set; }

        public virtual CombatEntry? CombatEntry { get; set; }
    }
}
