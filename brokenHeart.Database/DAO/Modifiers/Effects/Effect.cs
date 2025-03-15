using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Counters;

namespace brokenHeart.Database.DAO.Modifiers.Effects
{
    public class Effect : Modifier
    {
        [JsonConstructor]
        public Effect() { }

        //Per round
        public string Hp { get; set; } = "";

        //Total for the duration
        public int MaxTempHp { get; set; } = 0;
        public string Duration { get; set; } = "";

        public EffectCounter? EffectCounter { get; set; }

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }
    }
}
