using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using System.Text.Json.Serialization;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities.TargetAttackAbilities.SaveAttackAbilities
{
    public class SaveAttackAbility : TargetAttackAbility
    {
        [JsonConstructor]
        public SaveAttackAbility() { }
        public SaveAttackAbility(string name, string description, int range, Stat saveStat, string hp, List<Effect> effects,
            TargetType target, RoundReminder? roundReminder, int modifier, Stat abilityStat, List<Counter>? counters = null)
            : base(name, description, hp, range, effects, target, roundReminder, modifier, abilityStat, counters)
        {
            SaveStat = saveStat;
        }

        //Stat used for target roll
        public int SaveStatId { get; set; }
        public virtual Stat SaveStat { get; set; }
    }
}
