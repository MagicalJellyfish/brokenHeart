using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Effects.Injuries
{
    public class InjuryEffect : Effect
    {
        [JsonConstructor]
        public InjuryEffect() { }
        public InjuryEffect(string name, string @abstract, string duration, Bodypart bodypart, InjuryLevel injuryLevel, string description = "", int maxHp = 0, int movementSpeed = 0, int armor = 0, int evasion = 0, string hp = "", int maxTempHp = 0,
            List<StatValue>? statIncreases = null, List<Counter>? counters = null, RoundReminder? reminder = null, EffectCounter? effectCounter = null)
            : base(name, @abstract, description, description, maxHp, movementSpeed, armor, evasion, hp, maxTempHp, statIncreases, counters, reminder, effectCounter)
        {
            Bodypart = bodypart;
            InjuryLevel = injuryLevel;
        }

        public InjuryLevel InjuryLevel { get; set; }

        public int BodypartId { get; set; }
        public Bodypart Bodypart { get; set; }

        public int? CharacterInjuryId { get; set; }
        public virtual Character? CharacterInjury { get; set; }
    }
}
