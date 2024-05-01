using System.Text.Json.Serialization;
using brokenHeart.Entities.Abilities.Abilities;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Traits
{
    public class Trait : Modifier
    {
        [JsonConstructor]
        public Trait() { }

        public Trait(
            string name,
            string @abstract,
            string description,
            int maxHp,
            int movementSpeed,
            int armor,
            int evasion,
            List<StatValue> statIncreases,
            List<Counter> counters,
            RoundReminder? roundReminder = null
        )
            : base(
                name,
                @abstract,
                description,
                maxHp,
                movementSpeed,
                armor,
                evasion,
                statIncreases,
                counters,
                roundReminder
            )
        {
            Active = true;
        }

        public bool Active { get; set; }

        public int CharacterId { get; set; }
        public virtual Character? Character { get; set; }
    }
}
