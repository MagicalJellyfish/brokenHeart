using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities
{
    public class Ability
    {
        [JsonConstructor]
        public Ability() { }
        public Ability(string name, string description, RoundReminder? roundReminder, List<Counter>? counters = null)
        {
            Name = name;
            Description = description;
            RoundReminder = roundReminder;
            Counters = counters ?? new List<Counter>();

            ViewPosition = 0;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public ICollection<int>? CountersIds { get; set; } = new List<int>();
        public virtual ICollection<Counter> Counters { get; set; }

        public int? RoundReminderId { get; set; }
        public virtual RoundReminder? RoundReminder { get; set; }

        public int ViewPosition { get; set; }
    }
}
