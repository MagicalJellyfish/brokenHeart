using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities.TargetAttackAbilities
{
    public abstract class TargetAttackAbility : TargetAbility
    {
        [JsonConstructor]
        public TargetAttackAbility() { }
        public TargetAttackAbility(string name, string description, string hp, int range, List<Effect> effects,
            TargetType target, RoundReminder? roundReminder, int modifier, Stat abilityStat, List<Counter>? counters = null)
            : base(name, description, hp, range, effects, target, roundReminder, counters)
        {
            Modifier = modifier;
            AbilityStat = abilityStat;
        }

        public int Modifier { get; set; }

        //Stat used for Attack Roll
        public int AbilityStatId { get; set; }
        public virtual Stat AbilityStat { get; set; }

        public int? AbilityTypeId { get; set; }
        public virtual AbilityType? AbilityType { get; set; }
    }
}
