using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers.Effects.Injuries;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Characters
{
    public class Bodypart : IDao
    {
        [JsonConstructor]
        public Bodypart() { }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<BodypartCondition> BodypartConditions { get; set; } =
            new List<BodypartCondition>();
        public ICollection<InjuryEffect> InjuryEffects { get; set; } = new List<InjuryEffect>();
        public ICollection<InjuryEffectTemplate> InjuryEffectsTemplate { get; set; } =
            new List<InjuryEffectTemplate>();
    }
}
