using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Stats
{
    public class StatValue : IDao
    {
        [JsonConstructor]
        public StatValue() { }

        public int Id { get; set; }
        public int Value { get; set; }

        public int StatId { get; set; }
        public Stat Stat { get; set; }

        public int? ModifierId { get; set; }
        public Modifier? Modifier { get; set; }
        public int? ModifierTemplateId { get; set; }
        public ModifierTemplate? ModifierTemplate { get; set; }
        public int? CharacterId { get; set; }
        public Character? Character { get; set; }
    }
}
