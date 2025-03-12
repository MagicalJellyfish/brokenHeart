using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Database.DAO.Modifiers.Items
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
            List<AbilityTemplate>? abilityTemplates = null,
            List<CounterTemplate>? counterTemplates = null,
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
                abilityTemplates,
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
                StatIncreases.Select(x => x.Instantiate()).ToList(),
                AbilityTemplates.Select(x => x.Instantiate()).ToList(),
                CounterTemplates.Select(x => x.Instantiate()).ToList(),
                RoundReminderTemplate?.Instantiate()
            );
        }
    }
}
