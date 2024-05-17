using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Entities.Abilities.Abilities;
using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Items
{
    public class ItemTemplate : ModifierTemplate
    {
        [JsonConstructor]
        public ItemTemplate() { }

        public ItemTemplate(
            string name,
            string description = "",
            string @abstract = "",
            int maxHp = 0,
            int movementSpeed = 0,
            int armor = 0,
            int evasion = 0,
            int amount = 1,
            string unit = "",
            List<StatValue>? statIncreases = null,
            List<CounterTemplate>? counterTemplates = null,
            List<AbilityTemplate>? abilityTemplates = null,
            RoundReminderTemplate? reminderTemplate = null
        )
            : base(
                name,
                @abstract,
                description,
                maxHp,
                movementSpeed,
                armor,
                evasion,
                statIncreases,
                counterTemplates,
                reminderTemplate
            )
        {
            Amount = amount;
            Unit = unit;
            AbilityTemplates = abilityTemplates ?? new List<AbilityTemplate>();
        }

        public int Amount { get; set; }
        public string Unit { get; set; }

        [NotMapped]
        public ICollection<int>? AbilityTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<AbilityTemplate> AbilityTemplates { get; set; } =
            new List<AbilityTemplate>();

        [NotMapped]
        public ICollection<int>? CharacterTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();

        public Item Instantiate()
        {
            return new Item(
                Name,
                Abstract,
                Description,
                MaxHp,
                MovementSpeed,
                Armor,
                Evasion,
                Amount,
                Unit,
                AbilityTemplates.Select(x => x.Instantiate()).ToList(),
                StatIncreases.Select(x => x.Instantiate()).ToList(),
                CounterTemplates.Select(x => x.Instantiate()).ToList(),
                RoundReminderTemplate?.Instantiate()
            );
        }
    }
}
