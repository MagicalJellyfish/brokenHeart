using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.Modifiers.Effects;

namespace brokenHeart.Database.DAO.Abilities.Abilities
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
            int? maxUses = null,
            ReplenishType replenishType = ReplenishType.None,
            ICollection<Roll>? rolls = null,
            ICollection<EffectTemplate>? appliedEffectTemplates = null
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

            MaxUses = maxUses;
            ReplenishType = replenishType;

            AppliedEffectTemplates = appliedEffectTemplates;
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

        public int? MaxUses { get; set; }
        public ReplenishType ReplenishType { get; set; }

        [NotMapped]
        public ICollection<int>? RollsIds { get; set; } = new List<int>();
        public ICollection<Roll>? Rolls { get; set; } = new List<Roll>();

        [NotMapped]
        public ICollection<int>? AppliedEffectTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<EffectTemplate> AppliedEffectTemplates { get; set; } =
            new List<EffectTemplate>();

        [NotMapped]
        public ICollection<int>? ModifierTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<ModifierTemplate> ModifierTemplates { get; set; } =
            new List<ModifierTemplate>();

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
                MaxUses,
                ReplenishType,
                Rolls.Select(x => x.Instantiate()).ToList(),
                AppliedEffectTemplates
            );
        }
    }
}
