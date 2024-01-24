﻿using brokenHeart.Entities.Counters;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.RoundReminders
{
    public class RoundReminderTemplate
    {
        [JsonConstructor]
        public RoundReminderTemplate() { }
        public RoundReminderTemplate(string reminder, bool reminding = true)
        {
            Reminder = reminder;
            Reminding = reminding;
        }

        public int Id { get; set; }
        public bool Reminding { get; set; }
        public string Reminder { get; set; }

        [NotMapped]
        public ICollection<int>? CounterTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<CounterTemplate> CounterTemplates { get; set; } = new List<CounterTemplate>();
        [NotMapped]
        public ICollection<int>? ModifierTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<ModifierTemplate> ModifierTemplates { get; set; } = new List<ModifierTemplate>();

        public RoundReminder Instantiate()
        {
            return new RoundReminder(Reminder, Reminding);
        }
    }
}
