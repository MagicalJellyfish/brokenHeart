using brokenHeart.Database.DAO.Counters;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Counters;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search.Abilities;

namespace brokenHeart.Services.DataTransfer.Projection.Abilities
{
    internal class CounterProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Counter;

        private readonly ICounterSearchService _counterSearchService;

        public CounterProjectionService(ICounterSearchService counterSearchService)
        {
            _counterSearchService = counterSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Counter> counters = _counterSearchService.GetCounters(
                new CounterSearch() { Id = id }
            );

            if (counters.Count() == 0 || counters.Count() > 1)
            {
                return null;
            }

            return counters
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = (int)CounterField.Description,
                            Title = "Description",
                            Content = x.Description
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
                            FieldId = (int)CounterField.Name,
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String
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
                                        FieldId = (int)CounterField.Value,
                                        Title = "Value",
                                        Content = x.Value,
                                        Type = ElementView.FieldType.Number
                                    },
                                    new ElementView.Field()
                                    {
                                        FieldId = (int)CounterField.Max,
                                        Title = "Maximum Value",
                                        Content = x.Max,
                                        Type = ElementView.FieldType.Number
                                    }
                                }
                            },
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)CounterField.RoundBased,
                            Title = "Round-Based",
                            Content = x.RoundBased,
                            Type = ElementView.FieldType.Boolean
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
                                        }
                                    }
                                    : new List<ElementView.Relation.ElementRelationItem>()
                        },
                    }
                })
                .SingleOrDefault();
        }
    }
}
