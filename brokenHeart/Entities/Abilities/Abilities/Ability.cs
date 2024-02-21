using brokenHeart.Entities.Effects;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities
{
    public class Ability
    {
        [JsonConstructor]
        public Ability() { }

        public Ability(string name, string description, bool canInjure = false, TargetType? targetType = null, string? self = null, string? target = null, string? damage = null, ICollection<Roll>? rolls = null, ICollection<EffectTemplate>? effectTemplates = null)
        {
            Name = name;
            Description = description;
            CanInjure = canInjure;
            TargetType = targetType;
            Self = self;
            Target = target;
            Damage = damage;
            Rolls = rolls;
            EffectTemplates = effectTemplates;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public TargetType? TargetType { get; set; }

        public bool CanInjure { get; set; }
        public string? Self { get; set; }
        public string? Target { get; set; }
        public string? Damage { get; set; }

        [NotMapped]
        public ICollection<int>? RollsIds { get; set; }
        public virtual ICollection<Roll>? Rolls { get; set; }

        public ICollection<int>? EffectTemplatesIds { get; set; }
        public virtual ICollection<EffectTemplate>? EffectTemplates { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; }
    }
}
