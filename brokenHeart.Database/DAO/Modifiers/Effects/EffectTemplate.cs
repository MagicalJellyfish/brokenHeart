using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Database.DAO.Modifiers.Effects
{
    public class EffectTemplate : ModifierTemplate
    {
        [JsonConstructor]
        public EffectTemplate() { }

        public EffectTemplate(
            string name,
            string @abstract,
            string duration,
            string description = "",
            int maxHp = 0,
            int movementSpeed = 0,
            int armor = 0,
            int evasion = 0,
            string hp = "",
            int maxTempHp = 0,
            List<StatValue>? statIncreases = null,
            List<AbilityTemplate>? abilityTemplates = null,
            List<CounterTemplate>? counterTemplates = null,
            EffectCounterTemplate? effectCounterTemplate = null,
            RoundReminderTemplate? reminderTemplate = null
        )
            : base(
                name,
                description,
                @abstract,
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
            Hp = hp;
            MaxTempHp = maxTempHp;
            Duration = duration;
            EffectCounterTemplate = effectCounterTemplate;
        }

        //Per round
        public string Hp { get; set; } = "";

        //Total for the duration
        public int MaxTempHp { get; set; } = 0;
        public string Duration { get; set; } = "";

        public int? EffectCounterTemplateId { get; set; }
        public virtual EffectCounterTemplate? EffectCounterTemplate { get; set; }

        public virtual ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();

        public virtual ICollection<Ability> ApplyingAbilities { get; set; } = new List<Ability>();

        public virtual ICollection<AbilityTemplate> ApplyingAbilityTemplates { get; set; } =
            new List<AbilityTemplate>();

        public Effect Instantiate()
        {
            return new Effect(
                Name,
                Abstract,
                Duration,
                Description,
                MaxHp,
                MovementSpeed,
                Armor,
                Evasion,
                Hp,
                MaxTempHp,
                StatIncreases.Select(x => x.Instantiate()).ToList(),
                AbilityTemplates.Select(x => x.Instantiate()).ToList(),
                CounterTemplates.Select(x => x.Instantiate()).ToList(),
                RoundReminderTemplate?.Instantiate(),
                EffectCounterTemplate?.Instantiate()
            );
        }
    }
}
