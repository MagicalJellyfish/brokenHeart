using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Effects;

namespace brokenHeart.Entities.Abilities.Abilities
{
    public class AbilityTemplate
    {
        [JsonConstructor]
        public AbilityTemplate() { }

        public AbilityTemplate(
            string name,
            string description,
            bool canInjure = false,
            TargetType targetType = TargetType.None,
            string? self = null,
            string? target = null,
            string? damage = null,
            string? range = null,
            ICollection<Roll>? rolls = null,
            ICollection<EffectTemplate>? effectTemplates = null
        )
        {
            Name = name;
            Description = description;
            CanInjure = canInjure;
            TargetType = targetType;
            Self = self;
            Target = target;
            Damage = damage;
            Range = range;
            Rolls = rolls;
            EffectTemplates = effectTemplates;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public TargetType TargetType { get; set; }

        public bool CanInjure { get; set; }
        public string? Self { get; set; }
        public string? Target { get; set; }
        public string? Damage { get; set; }

        public string? Range { get; set; }

        [NotMapped]
        public ICollection<int>? RollsIds { get; set; } = new List<int>();
        public ICollection<Roll>? Rolls { get; set; } = new List<Roll>();

        [NotMapped]
        public ICollection<int>? EffectTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<EffectTemplate>? EffectTemplates { get; set; } =
            new List<EffectTemplate>();

        [NotMapped]
        public ICollection<int> CharacterTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();

        public Ability Instantiate()
        {
            return new Ability(
                Name,
                Description,
                null,
                CanInjure,
                TargetType,
                Self,
                Target,
                Damage,
                Range,
                Rolls.Select(x => x.Instantiate()).ToList(),
                EffectTemplates
            );
        }
    }
}
