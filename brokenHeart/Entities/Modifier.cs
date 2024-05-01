using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Entities.Abilities.Abilities;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities
{
    public abstract class Modifier
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
            List<StatValue>? statIncrease = null, /*List<Ability>? abilities = null,*/
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
            //Abilities = abilities ?? new List<Ability>();
            Counters = counters ?? new List<Counter>();
            RoundReminder = roundReminder;

            ViewPosition = 0;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abstract { get; set; }
        public int MaxHp { get; set; }
        public int MovementSpeed { get; set; }
        public int Armor { get; set; }
        public int Evasion { get; set; }

        [NotMapped]
        public ICollection<int>? StatIncreasesIds { get; set; } = new List<int>();
        public virtual ICollection<StatValue> StatIncreases { get; set; } = new List<StatValue>();

        [NotMapped]
        public ICollection<int>? CountersIds { get; set; } = new List<int>();
        public virtual ICollection<Counter> Counters { get; set; } = new List<Counter>();

        public virtual RoundReminder? RoundReminder { get; set; }

        public int ViewPosition { get; set; }
    }
}
