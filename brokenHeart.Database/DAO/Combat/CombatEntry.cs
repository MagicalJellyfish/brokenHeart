using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Combat
{
    public class CombatEntry : IDao
    {
        [JsonConstructor]
        public CombatEntry() { }

        public CombatEntry(
            int combatId,
            int characterId,
            int initRoll,
            int initStat,
            string shortcut
        )
        {
            CombatId = combatId;
            CharacterId = characterId;
            InitRoll = initRoll;
            Shortcut = shortcut;
        }

        public CombatEntry(int combatId, Event @event, int initRoll, string shortcut)
        {
            CombatId = combatId;
            Event = @event;
            InitRoll = initRoll;
            Shortcut = shortcut;
        }

        public int Id { get; set; }
        public int InitRoll { get; set; }
        public int InitStat { get; set; }
        public string Shortcut { get; set; }

        public int? CombatId { get; set; }
        public Combat? Combat { get; set; }

        public int? CharacterId { get; set; }
        public virtual Character? Character { get; set; }

        public int? EventId { get; set; }
        public virtual Event? Event { get; set; }
    }
}
