using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.RoundReminders
{
    internal interface IRoundReminderSearchService
    {
        public IQueryable<RoundReminder> GetRoundReminders(RoundReminderSearch search);
    }
}
