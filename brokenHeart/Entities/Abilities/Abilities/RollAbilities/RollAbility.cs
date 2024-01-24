using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities.RollAbilities
{
    public class RollAbility : Ability
    {
        [JsonConstructor]
        public RollAbility() { }
        public RollAbility(string name, string description, string roll, RoundReminder? roundReminder, List<Counter>? counters = null)
            : base(name, description, roundReminder, counters)
        {
            Roll = roll;
        }

        public string Roll { get; set; }
    }
}
