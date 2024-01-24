using brokenHeart.Entities.Stats;
using System.ComponentModel.DataAnnotations.Schema;

namespace brokenHeart.Entities.Combat
{
    public class Combat
    {
        public Combat()
        {
            Round = 1;
            CurrentTurn = 0;
        }

        public int Id { get; set; }
        public int Round { get; set; }
        public int CurrentTurn { get; set; }

        [NotMapped]
        public ICollection<int>? ParticipantsIds { get; set; } = new List<int>();
        public ICollection<CombatCharacterData> _participants = new List<CombatCharacterData>();
        public virtual ICollection<CombatCharacterData> Participants
        {
            get
            {
                return _participants;
            }
            set
            {
                _participants = (ICollection<CombatCharacterData>)value.OrderBy(x => x.InitRoll)
                    .ThenBy(x => ((StatValue)x.Character.Stats.Where(x => x.Stat == Constants.Stats.Dex)).Value);
            }
        }

        [NotMapped]
        public ICollection<int>? EventsIds { get; set; } = new List<int>();
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
