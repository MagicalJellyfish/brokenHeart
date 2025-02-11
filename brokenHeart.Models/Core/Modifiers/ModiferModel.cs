using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Models.Core.Abilities.Abilities;
using brokenHeart.Models.Core.Counters;
using brokenHeart.Models.Core.Stats;

namespace brokenHeart.Models.Core.Modifiers
{
    public class ModiferModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abstract { get; set; }
        public int MaxHp { get; set; }
        public int MovementSpeed { get; set; }
        public int Armor { get; set; }
        public int Evasion { get; set; }

        public List<StatValueModel> StatIncreases { get; set; } = new List<StatValueModel>();
        public List<CounterModel> Counters { get; set; } = new List<CounterModel>();
        public List<AbilityModel> Abilities { get; set; } = new List<AbilityModel>();

        public RoundReminder? RoundReminder { get; set; }

        public int ViewPosition { get; set; }
    }
}
