using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Entities.Abilities.Abilities;
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
            int amount,
            string unit,
            List<Ability> abilities,
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
            Amount = 1;
            Unit = "";
            Abilities = abilities;
        }

        public bool Equipped { get; set; }
        public int Amount { get; set; }
        public string Unit { get; set; }

        [NotMapped]
        public ICollection<int>? AbilitiesIds { get; set; } = new List<int>();
        public virtual ICollection<Ability> Abilities { get; set; } = new List<Ability>();

        public int CharacterId { get; set; }
        public virtual Character? Character { get; set; }
    }
}
