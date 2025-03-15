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

        public ModifierTemplate(
            string name,
            string description = "",
            string @abstract = "",
            int maxHp = 0,
            int movementSpeed = 0,
            int armor = 0,
            int evasion = 0,
            List<StatValue>? statIncrease = null,
            List<AbilityTemplate>? abilityTemplates = null,
            List<CounterTemplate>? counterTemplates = null,
            RoundReminderTemplate? reminderTemplate = null
        )
        {
            Name = name;
            Description = description;
            Abstract = @abstract;
            MaxHp = maxHp;
            MovementSpeed = movementSpeed;
            Armor = armor;
            Evasion = evasion;
            StatIncreases = statIncrease ?? new List<StatValue>();
            AbilityTemplates = abilityTemplates ?? new List<AbilityTemplate>();
            CounterTemplates = counterTemplates ?? new List<CounterTemplate>();
            RoundReminderTemplate = reminderTemplate;
        }

        public int Id { get; set; }
        public string Name { get; set; } = "New Modifier Template";
        public string Description { get; set; } = "";
        public string Abstract { get; set; } = "";
        public int MaxHp { get; set; } = 0;
        public int MovementSpeed { get; set; } = 0;
        public int Armor { get; set; } = 0;
        public int Evasion { get; set; } = 0;

        public virtual ICollection<StatValue> StatIncreases { get; set; } = new List<StatValue>();

        public virtual ICollection<CounterTemplate> CounterTemplates { get; set; } =
            new List<CounterTemplate>();

        public virtual ICollection<AbilityTemplate> AbilityTemplates { get; set; } =
            new List<AbilityTemplate>();

        public int? RoundReminderTemplateId { get; set; }
        public virtual RoundReminderTemplate? RoundReminderTemplate { get; set; }
    }
}
