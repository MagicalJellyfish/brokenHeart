using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.RoundReminders
{
    internal class RoundReminderTemplateProjectionService
        : IElementProjectionService,
            ITemplateProjectionService
    {
        public ElementType ProjectionType => ElementType.ReminderTemplate;

        private readonly IDaoSearchService _daoSearchService;

        public RoundReminderTemplateProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<RoundReminderTemplate> roundReminders =
                _daoSearchService.GetSingleElement<RoundReminderTemplate>(
                    new DaoSearch() { Id = id }
                );

            return roundReminders
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(RoundReminderTemplate.Reminder),
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
                            FieldId = nameof(RoundReminderTemplate.Reminding),
                            Title = "Reminding",
                            Content = x.Reminding,
                            Type = ElementView.FieldType.Boolean
                        },
                    },
                    Relations = new() { }
                })
                .SingleOrDefault();
        }

        public ElementList GetTemplateList()
        {
            return new ElementList()
            {
                Title = "Reminders",
                Type = ElementType.ReminderTemplate,
                ElementColumns = TemplateListModels.RoundReminderTemplateColumns,
                Elements = _daoSearchService
                    .GetElements<RoundReminderTemplate>()
                    .Where(x => x.Id > 23)
                    .Select(x => new TemplateListModels.RoundReminderTemplateModel()
                    {
                        Id = x.Id,
                        Reminder = x.Reminder
                    })
                    .Cast<dynamic>()
                    .ToList()
            };
        }
    }
}
