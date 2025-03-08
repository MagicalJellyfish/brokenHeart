using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search.RoundReminders;

namespace brokenHeart.Services.DataTransfer.Projection.RoundReminders
{
    internal class RoundReminderProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Reminder;

        private readonly IRoundReminderSearchService _roundReminderSearchService;

        public RoundReminderProjectionService(
            IRoundReminderSearchService roundReminderSearchService
        )
        {
            _roundReminderSearchService = roundReminderSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<RoundReminder> roundReminders =
                _roundReminderSearchService.GetRoundReminders(
                    new RoundReminderSearch() { Id = id }
                );

            if (roundReminders.Count() == 0 || roundReminders.Count() > 1)
            {
                return null;
            }

            return roundReminders
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text() { Title = "Reminding Text", Content = x.Reminder },
                    },
                    Fields = new()
                    {
                        new ElementView.Field()
                        {
                            Title = "Id",
                            Content = x.Id,
                            Type = ElementView.FieldType.Fixed
                        },
                        new ElementView.Field()
                        {
                            Title = "Reminding",
                            Content = x.Reminding,
                            Type = ElementView.FieldType.Boolean
                        },
                    },
                    Relations = new() { }
                })
                .SingleOrDefault();
        }
    }
}
