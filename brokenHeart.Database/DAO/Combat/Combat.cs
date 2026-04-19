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

        public ICollection<CombatEntry> Entries { get; set; } = new List<CombatEntry>();
    }
}
