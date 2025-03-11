using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.RoundReminders
{
    internal class RoundReminderProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Reminder;

        private readonly IDaoSearchService _daoSearchService;

        public RoundReminderProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<RoundReminder> roundReminders =
                _daoSearchService.GetSingleElement<RoundReminder>(new DaoSearch() { Id = id });

            return roundReminders
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(RoundReminder.Reminder),
                            Title = "Reminding Text",
                            Content = x.Reminder
                        },
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
                            FieldId = nameof(RoundReminder.Reminding),
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
