using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Modifiers
{
    public class ModifierTemplate : IDao, IElement, IStatValueElement
    {
        [JsonConstructor]
        public ModifierTemplate() { }

        public int Id { get; set; }
        public string Name { get; set; } = "New Modifier Template";
        public string Description { get; set; } = "";
        public string Abstract { get; set; } = "";
        public int MaxHp { get; set; } = 0;
        public int MovementSpeed { get; set; } = 0;
        public int Armor { get; set; } = 0;
        public int Evasion { get; set; } = 0;

        public ICollection<StatValue> StatIncreases { get; set; } = new List<StatValue>();

        public ICollection<CounterTemplate> CounterTemplates { get; set; } =
            new List<CounterTemplate>();

        public ICollection<AbilityTemplate> AbilityTemplates { get; set; } =
            new List<AbilityTemplate>();

        public int? RoundReminderTemplateId { get; set; }
        public RoundReminderTemplate? RoundReminderTemplate { get; set; }
    }
}
