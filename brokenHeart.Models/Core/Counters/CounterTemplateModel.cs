using brokenHeart.Models.Core.RoundReminders;

namespace brokenHeart.Models.Core.Counters
{
    public class CounterTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Max { get; set; }
        public bool RoundBased { get; set; }

        public RoundReminderTemplateModel? RoundReminderTemplate { get; set; }
    }
}
