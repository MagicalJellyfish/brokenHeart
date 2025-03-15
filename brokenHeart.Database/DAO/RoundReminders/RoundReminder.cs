using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.RoundReminders
{
    public class RoundReminder : IDao, IOrderableElement
    {
        [JsonConstructor]
        public RoundReminder() { }

        public int Id { get; set; }
        public bool Reminding { get; set; } = true;
        public string Reminder { get; set; } = "New Reminder";

        public int ViewPosition { get; set; }

        [ForeignKey("Character")]
        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        [ForeignKey("Counter")]
        public int? CounterId { get; set; }
        public Counter? Counter { get; set; }

        [ForeignKey("Modifier")]
        public int? ModifierId { get; set; }
        public Modifier? Modifier { get; set; }
    }
}
