using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;

namespace brokenHeart.Database.DAO.Modifiers.Effects
{
    public class EffectTemplate : ModifierTemplate
    {
        [JsonConstructor]
        public EffectTemplate() { }

        //Per round
        public string Hp { get; set; } = "";

        //Total for the duration
        public int MaxTempHp { get; set; } = 0;
        public string Duration { get; set; } = "";

        public int? EffectCounterTemplateId { get; set; }
        public EffectCounterTemplate? EffectCounterTemplate { get; set; }

        public ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();

        public ICollection<Ability> ApplyingAbilities { get; set; } = new List<Ability>();

        public ICollection<AbilityTemplate> ApplyingAbilityTemplates { get; set; } =
            new List<AbilityTemplate>();
    }
}
