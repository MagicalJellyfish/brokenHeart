using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities
{
    public class AbilityTemplate
    {
        [JsonConstructor]
        public AbilityTemplate() { }
        public AbilityTemplate(string name, string description = "", List<CounterTemplate>? counterTemplates = null, RoundReminderTemplate? roundReminderTemplate = null)
        {
            Name = name;
            Description = description;
            CounterTemplates = counterTemplates ?? new List<CounterTemplate>();
            ReminderTemplate = roundReminderTemplate;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public ICollection<int>? CounterTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<CounterTemplate> CounterTemplates { get; set; }

        public int? ReminderTemplateId { get; set; }
        public virtual RoundReminderTemplate? ReminderTemplate { get; set; }

        public Ability Instantiate()
        {
            RoundReminder? roundReminder = ReminderTemplate?.Instantiate();
            return new Ability(Name, Description, roundReminder, CounterTemplates.Select(x => x.Instantiate()).ToList());
        }
    }
}
