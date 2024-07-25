using System.Text.Json.Serialization;
using brokenHeart.Entities.Abilities.Abilities;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Effects
{
    public class Effect : Modifier
    {
        [JsonConstructor]
        public Effect() { }

        public Effect(
            string name,
            string @abstract,
            string duration,
            string description = "",
            int maxHp = 0,
            int movementSpeed = 0,
            int armor = 0,
            int evasion = 0,
            string hp = "",
            int maxTempHp = 0,
            List<StatValue>? statIncreases = null,
            List<Ability>? abilities = null,
            List<Counter>? counters = null,
            RoundReminder? reminder = null,
            EffectCounter? effectCounter = null
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
            Hp = hp;
            MaxTempHp = maxTempHp;
            Duration = duration;
            EffectCounter = effectCounter;
        }

        //Per round
        public string Hp { get; set; }

        //Total for the duration
        public int MaxTempHp { get; set; }
        public string Duration { get; set; }

        public virtual EffectCounter? EffectCounter { get; set; }

        public int? CharacterId { get; set; }
        public virtual Character? Character { get; set; }
    }
}
