using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.RoundReminders
{
    internal class RoundReminderSearchService : SearchService, IRoundReminderSearchService
    {
        public RoundReminderSearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<RoundReminder> GetRoundReminders(RoundReminderSearch search)
        {
            IQueryable<RoundReminder> reminders = _context.RoundReminders;

            if (search.Id != null)
            {
                reminders = reminders.Where(x => x.Id == search.Id);
            }

            return reminders;
        }
    }
}
