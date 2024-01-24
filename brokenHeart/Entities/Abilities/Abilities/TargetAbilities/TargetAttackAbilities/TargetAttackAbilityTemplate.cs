using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities.TargetAttackAbilities
{
    public abstract class TargetAttackAbilityTemplate : TargetAbilityTemplate
    {
        [JsonConstructor]
        public TargetAttackAbilityTemplate() { }
        public TargetAttackAbilityTemplate(string name, string description, int range, Stat abilityStat, string hp = "", List<EffectTemplate>? effects = null,
            TargetType target = TargetType.Single, List<CounterTemplate>? counterTemplates = null, RoundReminderTemplate? roundReminderTemplate = null, int modifier = 0)
            : base(name, description, range, hp, effects, target, counterTemplates, roundReminderTemplate)
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
