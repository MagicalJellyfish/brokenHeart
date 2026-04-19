using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection.RoundReminders
{
    internal class RoundReminderProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Reminder;

        private readonly BrokenDbContext _context;

        public RoundReminderProjectionService(BrokenDbContext context)
        {
            _context = context;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<RoundReminder> roundReminders = _context.RoundReminders.Where(x =>
                x.Id == id
            );

            return roundReminders
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(RoundReminder.Reminder),
                            Title = "Reminding Text",
                            Content = x.Reminder,
                        },
                    },
                    Fields = new()
                    {
                        new ElementView.Field()
                        {
                            Title = "Id",
                            Content = x.Id,
                            Type = ElementView.FieldType.Fixed,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(RoundReminder.Reminding),
                            Title = "Reminding",
                            Content = x.Reminding,
                            Type = ElementView.FieldType.Boolean,
                        },
                    },
                    Relations = new() { },
                })
                .SingleOrDefault();
        }
    }
}
