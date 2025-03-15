using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Modifiers
{
    public abstract class Modifier : IDao, IOrderableElement, IStatValueElement
    {
        [JsonConstructor]
        public Modifier() { }

        public int Id { get; set; }
        public string Name { get; set; } = "New Modifier";
        public string Description { get; set; } = "";
        public string Abstract { get; set; } = "";
        public int MaxHp { get; set; } = 0;
        public int MovementSpeed { get; set; } = 0;
        public int Armor { get; set; } = 0;
        public int Evasion { get; set; } = 0;

        public ICollection<StatValue> StatIncreases { get; set; } = new List<StatValue>();

        public ICollection<Counter> Counters { get; set; } = new List<Counter>();

        public ICollection<Ability> Abilities { get; set; } = new List<Ability>();

        public RoundReminder? RoundReminder { get; set; }

        public int ViewPosition { get; set; }
    }
}
