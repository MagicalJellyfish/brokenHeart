using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using System.Text.Json.Serialization;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities.TargetAttackAbilities.DirectAttackAbilities
{
    public class DirectAttackAbilityTemplate : TargetAttackAbilityTemplate
    {
        [JsonConstructor]
        public DirectAttackAbilityTemplate() { }
        public DirectAttackAbilityTemplate(string name, string description, int range, Stat abilityStat, string hp = "", List<EffectTemplate>? effects = null,
            TargetType target = TargetType.Single, bool armorPiercing = false, List<CounterTemplate>? counterTemplates = null,
            RoundReminderTemplate? roundReminderTemplate = null, int modifier = 0)
            : base(name, description, range, abilityStat, hp, effects, target, counterTemplates, roundReminderTemplate, modifier)
        {
            ArmorPiercing = armorPiercing;
        }

        public bool ArmorPiercing { get; set; }

        public new DirectAttackAbility Instantiate()
        {
            RoundReminder? roundReminder = ReminderTemplate?.Instantiate();
            return new DirectAttackAbility(Name, Description, Hp, Range, Effects.Select(x => x.Instantiate()).ToList(),
                Target, roundReminder, Modifier, AbilityStat, ArmorPiercing, CounterTemplates.Select(x => x.Instantiate()).ToList());
        }
    }
}
