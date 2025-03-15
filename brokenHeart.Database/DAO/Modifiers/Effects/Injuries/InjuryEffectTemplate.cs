using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Characters;

namespace brokenHeart.Database.DAO.Modifiers.Effects.Injuries
{
    public class InjuryEffectTemplate : EffectTemplate
    {
        [JsonConstructor]
        public InjuryEffectTemplate() { }

        public InjuryLevel InjuryLevel { get; set; } = InjuryLevel.None;

        public int BodypartId { get; set; }
        public Bodypart Bodypart { get; set; }
    }
}
