namespace brokenHeart.Models.Core.RoundReminders
{
    public class RoundReminderModel
    {
        public int Id { get; set; }
        public bool Reminding { get; set; }
        public string Reminder { get; set; }

        public int ViewPosition { get; set; }
    }
}
