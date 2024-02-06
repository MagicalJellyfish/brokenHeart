using brokenHeart.Entities.Characters;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Combat
{
    public class CombatEntry
    {
        [JsonConstructor]
        public CombatEntry() { }
        public CombatEntry(Character character, int initRoll, string shortcut)
        {
            Character = character;
            InitRoll = initRoll;
            Shortcut = shortcut;
        }

        public CombatEntry(Event @event, int initRoll, string shortcut)
        {
            Event = @event;
            InitRoll = initRoll;
            Shortcut = shortcut;
        }

        public int Id { get; set; }
        public int InitRoll { get; set; }
        public string Shortcut { get; set; }

        public int? CharacterId { get; set; }
        public virtual Character? Character { get; set; }

        public int? EventId { get; set; }
        public virtual Event? Event { get; set; }
    }
}
