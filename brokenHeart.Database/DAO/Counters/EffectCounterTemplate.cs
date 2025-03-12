using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers.Effects;

namespace brokenHeart.Database.DAO.Counters
{
    public class EffectCounterTemplate : CounterTemplate
    {
        [JsonConstructor]
        public EffectCounterTemplate() { }

        public EffectCounterTemplate(
            string name,
            int max,
            string description,
            bool roundBased,
            bool endEffect = true
        )
            : base(name, max, description, roundBased)
        {
            EndEffect = endEffect;
        }

        public bool EndEffect { get; set; }

        public virtual ICollection<EffectTemplate> EffectTemplates { get; set; } =
            new List<EffectTemplate>();

        public new EffectCounter Instantiate()
        {
            return new EffectCounter(Name, 0, Max, Description, RoundBased, EndEffect);
        }
    }
}
