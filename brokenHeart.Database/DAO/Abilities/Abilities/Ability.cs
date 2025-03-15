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

        public ICollection<Roll> Rolls { get; set; } = new List<Roll>();

        public ICollection<EffectTemplate> AppliedEffectTemplates { get; set; } =
            new List<EffectTemplate>();

        public int? ModifierId { get; set; }
        public Modifier? Modifier { get; set; }

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        public int ViewPosition { get; set; }
    }
}
