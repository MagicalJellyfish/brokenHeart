using brokenHeart.Database.DAO.Characters;

namespace brokenHeart.Models.Core.Modifiers.Effects.Injuries
{
    public class InjuryEffectModel : EffectModel
    {
        public InjuryLevel InjuryLevel { get; set; }

        public Bodypart Bodypart { get; set; }
    }
}
