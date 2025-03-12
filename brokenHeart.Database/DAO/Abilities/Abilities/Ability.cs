using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Abilities.Abilities
{
    public class Ability : IDao, IOrderableElement, IRolling
    {
        [JsonConstructor]
        public Ability() { }

        public Ability(
            string name,
            string description,
            string shortcut = "",
            bool canInjure = false,
            TargetType targetType = TargetType.None,
            string self = "",
            string target = "",
            string damage = "",
            string range = "",
            int maxUses = 0,
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
        public string Name { get; set; } = "New Ability";
        public string Abstract { get; set; } = "";
        public string Description { get; set; } = "";

        public string Shortcut { get; set; } = "";
        public TargetType TargetType { get; set; } = TargetType.Target;

        public bool CanInjure { get; set; } = true;
        public string Self { get; set; } = "";
        public string Target { get; set; } = "";
        public string Damage { get; set; } = "";

        public string Range { get; set; } = "";

        public int Uses { get; set; } = 0;
        public int MaxUses { get; set; } = 0;
        public ReplenishType ReplenishType { get; set; } = ReplenishType.None;

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
