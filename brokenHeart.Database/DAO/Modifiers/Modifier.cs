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

        public Modifier(
            string name,
            string description = "",
            string @abstract = "",
            int maxHp = 0,
            int movementSpeed = 0,
            int armor = 0,
            int evasion = 0,
            List<StatValue>? statIncrease = null,
            List<Ability>? abilities = null,
            List<Counter>? counters = null,
            RoundReminder? roundReminder = null
        )
        {
            Name = name;
            Description = description;
            Abstract = @abstract;
            MaxHp = maxHp;
            MovementSpeed = movementSpeed;
            Armor = armor;
            Evasion = evasion;
            StatIncreases = statIncrease ?? new List<StatValue>();
            Abilities = abilities ?? new List<Ability>();
            Counters = counters ?? new List<Counter>();
            RoundReminder = roundReminder;

            ViewPosition = 0;
        }

        public int Id { get; set; }
        public string Name { get; set; } = "New Modifier";
        public string Description { get; set; } = "";
        public string Abstract { get; set; } = "";
        public int MaxHp { get; set; } = 0;
        public int MovementSpeed { get; set; } = 0;
        public int Armor { get; set; } = 0;
        public int Evasion { get; set; } = 0;

        public virtual ICollection<StatValue> StatIncreases { get; set; } = new List<StatValue>();

        public virtual ICollection<Counter> Counters { get; set; } = new List<Counter>();

        public virtual ICollection<Ability> Abilities { get; set; } = new List<Ability>();

        public virtual RoundReminder? RoundReminder { get; set; }

        public int ViewPosition { get; set; }
    }
}
