using System.ComponentModel.DataAnnotations.Schema;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Combat
{
    public class Combat : IDao
    {
        public Combat()
        {
            Round = 1;
            CurrentTurn = -1;
            Active = true;
        }

        public int Id { get; set; }
        public int Round { get; set; }
        public int CurrentTurn { get; set; }
        public bool Active { get; set; }

        [NotMapped]
        public ICollection<int>? EntriesIds { get; set; } = new List<int>();
        public virtual ICollection<CombatEntry> Entries { get; set; } = new List<CombatEntry>();
    }
}
