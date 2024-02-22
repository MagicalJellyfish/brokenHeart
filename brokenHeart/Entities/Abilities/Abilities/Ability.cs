using brokenHeart.Entities.Effects;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities
{
    public class Ability
    {
        [JsonConstructor]
        public Ability() { }

        public Ability(string name, string description, string? shortcut = null, bool canInjure = false, TargetType? targetType = null, string? self = null, string? target = null, string? damage = null, string? range = null, ICollection<Roll>? rolls = null, ICollection<EffectTemplate>? effectTemplates = null)
        {
            Name = name;
            Description = description;

            if(shortcut != null)
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
            Rolls = rolls;
            EffectTemplates = effectTemplates;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Shortcut { get; set; }
        public TargetType? TargetType { get; set; }

        public bool CanInjure { get; set; }
        public string? Self { get; set; }
        public string? Target { get; set; }
        public string? Damage { get; set; }

        public string? Range { get; set; }

        [NotMapped]
        public ICollection<int>? RollsIds { get; set; } = new List<int>();
        public virtual ICollection<Roll>? Rolls { get; set; } = new List<Roll>();

        [NotMapped]
        public ICollection<int>? EffectTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<EffectTemplate>? EffectTemplates { get; set; } = new List<EffectTemplate>();

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }
    }
}
