using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers.Effects;

namespace brokenHeart.Database.DAO.Counters
{
    public class EffectCounter : Counter
    {
        [JsonConstructor]
        public EffectCounter() { }

        public bool EndEffect { get; set; } = true;

        public int EffectId { get; set; }
        public Effect? Effect { get; set; }
    }
}
