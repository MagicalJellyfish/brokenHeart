using brokenHeart.Entities.Effects;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Counters
{
    public class EffectCounterTemplate : CounterTemplate
    {
        [JsonConstructor]
        public EffectCounterTemplate() { }
        public EffectCounterTemplate(string name, int max, string description, bool roundBased,  bool endEffect = true) 
            : base(name, max, description, roundBased)
        {
            EndEffect = endEffect;
        }

        public bool EndEffect { get; set; }

        [NotMapped]
        public ICollection<int> EffectTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<EffectTemplate> EffectTemplates { get; set; } = new List<EffectTemplate>();

        public new EffectCounter Instantiate()
        {
            return new EffectCounter(Name, 0, Max, Description, RoundBased, EndEffect);
        }
    }
}
