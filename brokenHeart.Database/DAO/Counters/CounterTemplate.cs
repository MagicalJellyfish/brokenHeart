using System.Text.Json.Serialization;
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

        public int Id { get; set; }
        public string Name { get; set; } = "New Counter Template";
        public string Description { get; set; } = "";
        public int Max { get; set; } = 0;
        public bool RoundBased { get; set; } = false;

        public int? RoundReminderTemplateId { get; set; }
        public RoundReminderTemplate? RoundReminderTemplate { get; set; }

        public ICollection<ModifierTemplate> ModifierTemplates { get; set; } =
            new List<ModifierTemplate>();

        public ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();
    }
}
