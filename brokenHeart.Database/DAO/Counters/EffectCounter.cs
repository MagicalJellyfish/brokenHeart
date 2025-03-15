using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers.Effects;

namespace brokenHeart.Database.DAO.Counters
{
    public class EffectCounter : Counter
    {
        [JsonConstructor]
        public EffectCounter() { }

        public EffectCounter(
            string name,
            int value,
            int max,
            string description,
            bool roundBased,
            bool endEffect = true
        )
            : base(name, max, description, roundBased)
        {
            EndEffect = endEffect;
        }

        public bool EndEffect { get; set; } = true;

        [ForeignKey("Effect")]
        public int EffectId { get; set; }
        public virtual Effect? Effect { get; set; }
    }
}
