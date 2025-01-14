using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Database.DAO.Modifiers.Items
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
            List<StatValue> statIncreases,
            List<Ability> abilities,
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
                abilities,
                counters,
                reminder
            )
        {
            Equipped = false;
            Amount = amount;
            Unit = unit;
        }

        public bool Equipped { get; set; }
        public int Amount { get; set; }
        public string Unit { get; set; }

        public int CharacterId { get; set; }
        public virtual Character? Character { get; set; }
    }
}
