using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers.Effects.Injuries;

namespace brokenHeart.Database.DAO.Characters
{
    public class Bodypart
    {
        [JsonConstructor]
        public Bodypart() { }

        public Bodypart(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BodypartCondition> BodypartConditions { get; set; } =
            new List<BodypartCondition>();
        public virtual ICollection<InjuryEffect> InjuryEffects { get; set; } =
            new List<InjuryEffect>();
        public virtual ICollection<InjuryEffectTemplate> InjuryEffectsTemplate { get; set; } =
            new List<InjuryEffectTemplate>();
    }
}
