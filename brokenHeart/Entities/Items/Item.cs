using System.Text.Json.Serialization;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Items
{
    public class Item : Modifier
    {
        [JsonConstructor]
        public Item() { }

        public Item(
            string name,
            string @abstract,
            string description,
            int maxHp,
            int movementSpeed,
            int armor,
            int evasion,
            List<StatValue> statIncreases,
            List<Counter> counters,
            RoundReminder? reminder
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
                reminder
            )
        {
            Equipped = false;
        }

        public bool Equipped { get; set; }

        public int CharacterId { get; set; }
        public virtual Character? Character { get; set; }
    }
}
