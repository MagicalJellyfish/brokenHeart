using brokenHeart.Database.DAO.Counters;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Counters
{
    internal class EffectCounterProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.EffectCounter;

        private readonly IDaoSearchService _daoSearchService;

        public EffectCounterProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<EffectCounter> counters = _daoSearchService.GetSingleElement<EffectCounter>(
                new DaoSearch() { Id = id }
            );

            return counters
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(EffectCounter.Description),
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
                            FieldId = nameof(EffectCounter.Name),
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
                                        FieldId = nameof(EffectCounter.Value),
                                        Title = "Value",
                                        Content = x.Value,
                                        Type = ElementView.FieldType.Number
                                    },
                                    new ElementView.Field()
                                    {
                                        FieldId = nameof(EffectCounter.Max),
                                        Title = "Maximum Value",
                                        Content = x.Max,
                                        Type = ElementView.FieldType.Number
                                    }
                                }
                            },
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectCounter.RoundBased),
                            Title = "Round-Based",
                            Content = x.RoundBased,
                            Type = ElementView.FieldType.Boolean
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectCounter.EndEffect),
                            Title = "Ends Effect",
                            Content = x.EndEffect,
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
