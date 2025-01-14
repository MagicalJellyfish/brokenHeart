using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.RoundReminders;

namespace brokenHeart.Database.DAO.Counters
{
    public class Counter
    {
        [JsonConstructor]
        public Counter() { }

        public Counter(
            string name,
            int max,
            string description,
            bool roundBased = true,
            RoundReminder? reminder = null
        )
        {
            Name = name;
            Max = max;
            Description = description;
            RoundBased = roundBased;
            RoundReminder = reminder;

            Value = 0;
            ViewPosition = 0;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public int Max { get; set; }
        public bool RoundBased { get; set; }

        public virtual RoundReminder? RoundReminder { get; set; }

        public int ViewPosition { get; set; }

        public int? CharacterId { get; set; }
        public virtual Character? Character { get; set; }
        public int? ModifierId { get; set; }
        public virtual Modifier? Modifier { get; set; }
    }
}
