using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using brokenHeart.Entities.RoundReminders;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities
{
    public class TargetAbility : Ability
    {
        [JsonConstructor]
        public TargetAbility() { }
        public TargetAbility(string name, string description, string hp, int range, List<Effect> effects,
            TargetType target, RoundReminder? roundReminder, List<Counter>? counters = null)
            : base(name, description, roundReminder, counters)
        {
            Hp = hp;
            Range = range;
            Effects = effects ?? new List<Effect>();
            Target = target;
        }

        public string Hp { get; set; }
        public int Range { get; set; }

        [NotMapped]
        public ICollection<int>? EffectsIds { get; set; } = new List<int>();
        public virtual ICollection<Effect> Effects { get; set; }

        public TargetType Target { get; set; }
    }
}
