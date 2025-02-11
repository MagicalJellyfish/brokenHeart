using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Models.Core.Modifiers.Effects;

namespace brokenHeart.Models.Core.Abilities.Abilities
{
    public class AbilityModel
    {
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

        public List<RollModel> Rolls { get; set; } = new List<RollModel>();

        public List<EffectTemplateModel> AppliedEffectTemplates { get; set; } =
            new List<EffectTemplateModel>();

        public int ViewPosition { get; set; }
    }
}
