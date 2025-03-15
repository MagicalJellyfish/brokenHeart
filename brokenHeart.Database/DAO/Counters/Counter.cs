using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Counters
{
    public class Counter : IDao, IOrderableElement
    {
        [JsonConstructor]
        public Counter() { }

        public int Id { get; set; }
        public string Name { get; set; } = "New Counter";
        public string Description { get; set; } = "";
        public int Value { get; set; } = 0;
        public int Max { get; set; } = 0;
        public bool RoundBased { get; set; } = false;

        public RoundReminder? RoundReminder { get; set; }

        public int ViewPosition { get; set; }

        public int? DeathCountCharacterId { get; set; }
        public Character? DeathCountCharacter { get; set; }

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }
        public int? ModifierId { get; set; }
        public Modifier? Modifier { get; set; }
    }
}
