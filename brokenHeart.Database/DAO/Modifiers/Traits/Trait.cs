using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Database.DAO.Modifiers.Traits
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
            List<Ability> abilities,
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
                abilities,
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
