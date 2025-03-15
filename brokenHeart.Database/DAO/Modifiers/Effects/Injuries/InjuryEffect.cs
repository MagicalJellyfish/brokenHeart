using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Characters;

namespace brokenHeart.Database.DAO.Modifiers.Effects.Injuries
{
    public class InjuryEffect : Effect
    {
        [JsonConstructor]
        public InjuryEffect() { }

        public InjuryLevel InjuryLevel { get; set; } = InjuryLevel.None;

        public int BodypartId { get; set; }
        public Bodypart Bodypart { get; set; }

        public int? CharacterInjuryId { get; set; }
        public Character? CharacterInjury { get; set; }
    }
}
