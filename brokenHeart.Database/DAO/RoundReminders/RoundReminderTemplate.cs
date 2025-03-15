using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.RoundReminders
{
    public class RoundReminderTemplate : IDao, IElement
    {
        [JsonConstructor]
        public RoundReminderTemplate() { }

        public int Id { get; set; }
        public bool Reminding { get; set; } = true;
        public string Reminder { get; set; } = "New Reminder Template";

        public ICollection<CounterTemplate> CounterTemplates { get; set; } =
            new List<CounterTemplate>();

        public ICollection<ModifierTemplate> ModifierTemplates { get; set; } =
            new List<ModifierTemplate>();

        public ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();
    }
}
