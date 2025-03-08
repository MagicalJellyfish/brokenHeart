using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers;

namespace brokenHeart.Database.DAO.RoundReminders
{
    public class RoundReminder
    {
        [JsonConstructor]
        public RoundReminder() { }

        public RoundReminder(string reminder, bool reminding = true)
        {
            Reminding = reminding;
            Reminder = reminder;

            ViewPosition = 0;
        }

        public int Id { get; set; }
        public bool Reminding { get; set; } = true;
        public string Reminder { get; set; } = "New Reminder";

        public int ViewPosition { get; set; }

        [ForeignKey("Character")]
        public int? CharacterId { get; set; }
        public virtual Character? Character { get; set; }

        [ForeignKey("Counter")]
        public int? CounterId { get; set; }
        public virtual Counter? Counter { get; set; }

        [ForeignKey("Modifier")]
        public int? ModifierId { get; set; }
        public virtual Modifier? Modifier { get; set; }
    }
}
