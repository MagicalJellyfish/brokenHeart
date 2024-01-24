using brokenHeart.Entities.Abilities.Abilities;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities.RollAbilities
{
    public class RollAbilityTemplate : AbilityTemplate
    {
        [JsonConstructor]
        public RollAbilityTemplate() { }
        public RollAbilityTemplate(string name, string description, string roll, RoundReminderTemplate? roundReminder, List<CounterTemplate>? counters = null)
            : base(name, description, counters, roundReminder)
        {
            Roll = roll;
        }

        public string Roll { get; set; }
    }
}
