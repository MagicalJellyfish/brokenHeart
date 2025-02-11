using brokenHeart.Models.Core.Abilities.Abilities;
using brokenHeart.Models.Core.Counters;

namespace brokenHeart.Models.Core.Modifiers.Effects
{
    public class EffectTemplateModel : ModifierTemplateModel
    { //Per round
        public string Hp { get; set; }

        //Total for the duration
        public int MaxTempHp { get; set; }
        public string Duration { get; set; }

        public EffectCounterTemplateModel? EffectCounterTemplate { get; set; }

        public List<AbilityTemplateModel> ApplyingAbilityTemplates { get; set; } =
            new List<AbilityTemplateModel>();
    }
}
