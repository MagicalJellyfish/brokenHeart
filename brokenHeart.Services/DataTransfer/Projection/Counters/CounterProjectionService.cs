using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection.Abilities
{
    internal class CounterProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Counter;

        private readonly BrokenDbContext _context;

        public CounterProjectionService(BrokenDbContext context)
        {
            _context = context;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Counter> counters = _context.Counters.Where(x => x.Id == id);

            return counters
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(Counter.Description),
                            Title = "Description",
                            Content = x.Description,
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
                            FieldId = nameof(Counter.Name),
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String,
                        },
                        new ElementView.Field()
                        {
                            Title = "Value",
                            Type = ElementView.FieldType.Multi,
                            Content = new ElementView.Field.MultiField()
                            {
                                Separator = "/",
                                Fields = new()
                                {
                                    new ElementView.Field()
                                    {
                                        FieldId = nameof(Counter.Value),
                                        Title = "Value",
                                        Content = x.Value,
                                        Type = ElementView.FieldType.Number,
                                    },
                                    new ElementView.Field()
                                    {
                                        FieldId = nameof(Counter.Max),
                                        Title = "Max Value",
                                        Content = x.Max,
                                        Type = ElementView.FieldType.Number,
                                    },
                                },
                            },
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Counter.RoundBased),
                            Title = "Round-Based",
                            Content = x.RoundBased,
                            Type = ElementView.FieldType.Boolean,
                        },
                    },
                    Relations = new()
                    {
                        new ElementView.Relation()
                        {
                            Title = "Reminders",
                            RelationType = ElementView.RelationType.SingleElement,
                            ElementType = ElementType.Reminder,
                            RelationItems =
                                x.RoundReminder != null
                                    ? new List<ElementView.Relation.ElementRelationItem>()
                                    {
                                        new ElementView.Relation.ElementRelationItem()
                                        {
                                            Id = x.RoundReminder.Id,
                                            Name = x.RoundReminder.Reminder,
                                        },
                                    }
                                    : new List<ElementView.Relation.ElementRelationItem>(),
                        },
                    },
                })
                .SingleOrDefault();
        }
    }
}
