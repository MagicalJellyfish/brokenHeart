using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Database.DAO.Modifiers
{
    public class ModifierTemplate
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
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abstract { get; set; }
        public int MaxHp { get; set; }
        public int MovementSpeed { get; set; }
        public int Armor { get; set; }
        public int Evasion { get; set; }

        [NotMapped]
        public ICollection<int>? StatIncreasesIds { get; set; } = new List<int>();
        public virtual ICollection<StatValue> StatIncreases { get; set; } = new List<StatValue>();

        [NotMapped]
        public ICollection<int>? CounterTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<CounterTemplate> CounterTemplates { get; set; } =
            new List<CounterTemplate>();

        [NotMapped]
        public ICollection<int>? AbilityTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<AbilityTemplate> AbilityTemplates { get; set; } =
            new List<AbilityTemplate>();

        public int? RoundReminderTemplateId { get; set; }
        public virtual RoundReminderTemplate? RoundReminderTemplate { get; set; }
    }
}
