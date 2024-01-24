using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using brokenHeart.Entities.RoundReminders;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities
{
    public class TargetAbilityTemplate : AbilityTemplate
    {
        [JsonConstructor]
        public TargetAbilityTemplate() { }
        public TargetAbilityTemplate(string name, string description, int range, string hp = "", List<EffectTemplate>? effects = null,
            TargetType target = TargetType.Single, List<CounterTemplate>? counterTemplates = null, RoundReminderTemplate? roundReminderTemplate = null)
            : base(name, description, counterTemplates, roundReminderTemplate)
        {
            Hp = hp;
            Range = range;
            Effects = effects ?? new List<EffectTemplate>();
            Target = target;
        }

        public string Hp { get; set; }
        public int Range { get; set; }

        [NotMapped]
        public ICollection<int>? EffectsIds { get; set; } = new List<int>();
        public virtual ICollection<EffectTemplate> Effects { get; set; } = new List<EffectTemplate>();

        public TargetType Target { get; set; }



        public new TargetAbility Instantiate()
        {
            RoundReminder? roundReminder = ReminderTemplate == null ? null : ReminderTemplate.Instantiate();
            return new TargetAbility(Name, Description, Hp, Range, Effects.Select(x => x.Instantiate()).ToList(), Target,
                roundReminder, CounterTemplates.Select(x => x.Instantiate()).ToList());
        }
    }
}
