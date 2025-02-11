using brokenHeart.Models.Core.Counters;

namespace brokenHeart.Models.Core.Modifiers.Effects
{
    public class EffectModel : ModiferModel
    {
        //Per round
        public string Hp { get; set; }

        //Total for the duration
        public int MaxTempHp { get; set; }
        public string Duration { get; set; }

        public EffectCounterModel EffectCounter { get; set; }
    }
}
