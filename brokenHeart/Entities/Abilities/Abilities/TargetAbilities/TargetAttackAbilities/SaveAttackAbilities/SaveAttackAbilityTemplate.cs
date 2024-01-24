using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using System.Text.Json.Serialization;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities.TargetAttackAbilities.SaveAttackAbilities
{
    public class SaveAttackAbilityTemplate : TargetAttackAbilityTemplate
    {
        [JsonConstructor]
        public SaveAttackAbilityTemplate() { }
        public SaveAttackAbilityTemplate(string name, string description, int range, int modifier, Stat abilityStat, Stat saveStat, string hp = "", List<EffectTemplate>? effects = null,
            TargetType target = TargetType.Single, List<CounterTemplate>? counterTemplates = null,
            RoundReminderTemplate? roundReminderTemplate = null)
            : base(name, description, range, abilityStat, hp, effects, target, counterTemplates, roundReminderTemplate, modifier)
        {
            SaveStat = saveStat;
        }

        //Stat used for target roll
        public int SaveStatId { get; set; }
        public virtual Stat SaveStat { get; set; }

        public new SaveAttackAbility Instantiate()
        {
            RoundReminder? roundReminder = ReminderTemplate?.Instantiate();
            return new SaveAttackAbility(Name, Description, Range, SaveStat, Hp, Effects.Select(x => x.Instantiate()).ToList(), Target,
                roundReminder, Modifier, AbilityStat, CounterTemplates.Select(x => x.Instantiate()).ToList());
        }
    }
}
