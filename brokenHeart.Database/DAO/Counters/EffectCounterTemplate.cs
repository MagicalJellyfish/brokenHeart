using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers.Effects;

namespace brokenHeart.Database.DAO.Counters
{
    public class EffectCounterTemplate : CounterTemplate
    {
        [JsonConstructor]
        public EffectCounterTemplate() { }

        public bool EndEffect { get; set; } = true;

        public ICollection<EffectTemplate> EffectTemplates { get; set; } =
            new List<EffectTemplate>();
    }
}
