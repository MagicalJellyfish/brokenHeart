﻿using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Counters
{
    public class CounterTemplate : IDao, IElement
    {
        [JsonConstructor]
        public CounterTemplate() { }

        public CounterTemplate(
            string name,
            int max,
            string description,
            bool roundBased = true,
            RoundReminderTemplate? reminderTemplate = null
        )
        {
            Name = name;
            Max = max;
            RoundBased = roundBased;
            Description = description;
            RoundReminderTemplate = reminderTemplate;
        }

        public int Id { get; set; }
        public string Name { get; set; } = "New Counter Template";
        public string Description { get; set; } = "";
        public int Max { get; set; } = 0;
        public bool RoundBased { get; set; } = false;

        public int? RoundReminderTemplateId { get; set; }
        public virtual RoundReminderTemplate? RoundReminderTemplate { get; set; }

        public virtual ICollection<ModifierTemplate> ModifierTemplates { get; set; } =
            new List<ModifierTemplate>();

        public virtual ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();

        public Counter Instantiate()
        {
            RoundReminder? reminder =
                RoundReminderTemplate != null ? RoundReminderTemplate.Instantiate() : null;
            return new Counter(Name, Max, Description, RoundBased, reminder);
        }
    }
}
