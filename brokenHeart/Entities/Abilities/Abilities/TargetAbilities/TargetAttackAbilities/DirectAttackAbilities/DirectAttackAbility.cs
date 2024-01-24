using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using System.Text.Json.Serialization;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities.TargetAttackAbilities.DirectAttackAbilities
{
    public class DirectAttackAbility : TargetAttackAbility
    {
        [JsonConstructor]
        public DirectAttackAbility() { }
        public DirectAttackAbility(string name, string description, string hp, int range, List<Effect> effects,
            TargetType target, RoundReminder? roundReminder, int modifier, Stat abilityStat, bool armorPiercing, List<Counter>? counters = null)
            : base(name, description, hp, range, effects, target, roundReminder, modifier, abilityStat, counters)
        {
            ArmorPiercing = armorPiercing;
        }
        public bool ArmorPiercing { get; set; }
    }
}
