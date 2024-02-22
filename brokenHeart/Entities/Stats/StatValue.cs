using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Stats
{
    public class StatValue
    {
        [JsonConstructor]
        public StatValue() { }
        public StatValue(Stat stat, int value)
        {
            Stat = stat;
            Value = value;
        }

        public StatValue(int statId, int value)
        {
            StatId = statId;
            Value = value;
        }

        public int Id { get; set; }
        public int Value { get; set; }

        public int StatId { get; set; }
        public virtual Stat? Stat { get; set; }

        public int? ModifierId { get; set; }
        public virtual Modifier? Modifier { get; set; }
        public int? ModifierTemplateId { get; set; }
        public virtual ModifierTemplate? ModifierTemplate { get; set; }
        public int? CharacterId { get; set; }
        public virtual Character? Character { get; set; }

        public StatValue Instantiate()
        {
            return new StatValue(StatId, Value);
        }
    }
}
