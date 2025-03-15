using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Abilities.Abilities
{
    public class AbilityTemplate : IDao, IElement, IRolling
    {
        [JsonConstructor]
        public AbilityTemplate() { }

        public int Id { get; set; }
        public string Name { get; set; } = "New Ability Template";
        public string Abstract { get; set; } = "";
        public string Description { get; set; } = "";
        public string Shortcut { get; set; } = "";

        public TargetType TargetType { get; set; } = TargetType.Target;

        public bool CanInjure { get; set; } = true;
        public string Self { get; set; } = "";
        public string Target { get; set; } = "";
        public string Damage { get; set; } = "";

        public string Range { get; set; } = "";

        public int MaxUses { get; set; } = 0;
        public ReplenishType ReplenishType { get; set; } = ReplenishType.None;

        public ICollection<Roll> Rolls { get; set; } = new List<Roll>();

        public ICollection<EffectTemplate> AppliedEffectTemplates { get; set; } =
            new List<EffectTemplate>();

        public ICollection<ModifierTemplate> ModifierTemplates { get; set; } =
            new List<ModifierTemplate>();

        public ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();
    }
}
