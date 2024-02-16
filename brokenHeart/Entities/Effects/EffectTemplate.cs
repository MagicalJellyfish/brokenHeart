using brokenHeart.Entities.Counters;
using System.Text.Json.Serialization;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Effects
{
    public class EffectTemplate : ModifierTemplate
    {
        [JsonConstructor]
        public EffectTemplate() { }
        public EffectTemplate(string name, string @abstract, string duration, string description = "", int maxHp = 0, int movementSpeed = 0, int armor = 0, int evasion = 0, string hp = "", int maxTempHp = 0,
            List<StatValue>? statIncreases = null, List<CounterTemplate>? counterTemplates = null, 
            EffectCounterTemplate? effectCounterTemplate = null, RoundReminderTemplate? reminderTemplate = null)
            : base(name, @abstract, description, maxHp, movementSpeed, armor, evasion,
                  statIncreases, counterTemplates, reminderTemplate)
        {
            Hp = hp;
            MaxTempHp = maxTempHp;
            Duration = duration;
            EffectCounterTemplate = effectCounterTemplate;
        }

        //Per round
        public string Hp { get; set; }
        //Total for the duration
        public int MaxTempHp { get; set; }
        public string Duration { get; set; }

        public int? EffectCounterTemplateId { get; set; }
        public virtual EffectCounterTemplate? EffectCounterTemplate { get; set; }

        public Effect Instantiate()
        {
            return new Effect(Name, Abstract, Duration, Description, MaxHp, MovementSpeed, Armor, Evasion, Hp, MaxTempHp, StatIncreases.ToList(),
                    CounterTemplates.Select(x => x.Instantiate()).ToList(), RoundReminderTemplate?.Instantiate(), EffectCounterTemplate?.Instantiate());
        }
    }
}
