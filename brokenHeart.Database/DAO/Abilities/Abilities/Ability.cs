using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.Modifiers.Effects;

namespace brokenHeart.Database.DAO.Abilities.Abilities
{
    public class Ability
    {
        [JsonConstructor]
        public Ability() { }

        public Ability(
            string name,
            string description,
            string? shortcut = null,
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

            if (shortcut != null)
            {
                Shortcut = shortcut;
            }
            else
            {
                Shortcut = name;
            }

            CanInjure = canInjure;
            TargetType = targetType;
            Self = self;
            Target = target;
            Damage = damage;
            Range = range;

            Uses = maxUses;
            MaxUses = maxUses;
            ReplenishType = replenishType;

            Rolls = rolls;
            AppliedEffectTemplates = appliedEffectTemplates;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Shortcut { get; set; }
        public TargetType TargetType { get; set; }

        public bool CanInjure { get; set; }
        public string? Self { get; set; }
        public string? Target { get; set; }
        public string? Damage { get; set; }

        public string? Range { get; set; }

        public int? Uses { get; set; }
        public int? MaxUses { get; set; }
        public ReplenishType ReplenishType { get; set; }

        [NotMapped]
        public ICollection<int>? RollsIds { get; set; } = new List<int>();
        public virtual ICollection<Roll>? Rolls { get; set; } = new List<Roll>();

        [NotMapped]
        public ICollection<int>? AppliedEffectTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<EffectTemplate>? AppliedEffectTemplates { get; set; } =
            new List<EffectTemplate>();

        public int? ModifierId { get; set; }
        public Modifier? Modifier { get; set; }

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        public int ViewPosition { get; set; }
    }
}
