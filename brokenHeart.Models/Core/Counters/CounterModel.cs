using brokenHeart.Database.DAO.RoundReminders;

namespace brokenHeart.Models.Core.Counters
{
    public class CounterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public int Max { get; set; }
        public bool RoundBased { get; set; }

        public RoundReminder? RoundReminder { get; set; }

        public int ViewPosition { get; set; }
    }
}
