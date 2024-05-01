using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Entities.Abilities.Abilities;
using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Traits
{
    public class TraitTemplate : ModifierTemplate
    {
        [JsonConstructor]
        public TraitTemplate() { }

        public TraitTemplate(
            string name,
            string @abstract,
            string description = "",
            int maxHp = 0,
            int movementSpeed = 0,
            int armor = 0,
            int evasion = 0,
            List<StatValue>? statIncreases = null,
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
                counterTemplates,
                reminderTemplate
            ) { }

        [NotMapped]
        public ICollection<int>? CharacterTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();

        public Trait Instantiate()
        {
            return new Trait(
                Name,
                Abstract,
                Description,
                MaxHp,
                MovementSpeed,
                Armor,
                Evasion,
                StatIncreases.Select(x => x.Instantiate()).ToList(),
                CounterTemplates.Select(x => x.Instantiate()).ToList(),
                RoundReminderTemplate?.Instantiate()
            );
        }
    }
}
