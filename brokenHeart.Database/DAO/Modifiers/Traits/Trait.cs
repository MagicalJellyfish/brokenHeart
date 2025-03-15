using System.Text.Json.Serialization;

namespace brokenHeart.Database.DAO.Modifiers.Traits
{
    public class Trait : Modifier
    {
        [JsonConstructor]
        public Trait() { }

        public bool Active { get; set; } = true;

        public int CharacterId { get; set; }
        public Character? Character { get; set; }
    }
}
