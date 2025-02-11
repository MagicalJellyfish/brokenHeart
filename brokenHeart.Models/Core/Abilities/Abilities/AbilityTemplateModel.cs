using brokenHeart.Database.DAO.Abilities;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Modifiers.Effects;

namespace brokenHeart.Models.Core.Abilities.Abilities
{
    public class AbilityTemplateModel
    {
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

        public List<Roll> Rolls { get; set; } = new List<Roll>();

        public List<EffectTemplate> AppliedEffectTemplates { get; set; } =
            new List<EffectTemplate>();
    }
}
