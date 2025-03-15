using brokenHeart.Database.DAO.Counters;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Counters
{
    internal class EffectCounterTemplateProjectionService
        : IElementProjectionService,
            ITemplateProjectionService
    {
        public ElementType ProjectionType => ElementType.EffectCounterTemplate;

        private readonly IDaoSearchService _daoSearchService;

        public EffectCounterTemplateProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<EffectCounterTemplate> counters =
                _daoSearchService.GetSingleElement<EffectCounterTemplate>(
                    new DaoSearch() { Id = id }
                );

            return counters
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(EffectCounterTemplate.Description),
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
                            FieldId = nameof(EffectCounterTemplate.Name),
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectCounterTemplate.Max),
                            Title = "Maximum Value",
                            Content = x.Max,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectCounterTemplate.RoundBased),
                            Title = "Round-Based",
                            Content = x.RoundBased,
                            Type = ElementView.FieldType.Boolean
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectCounterTemplate.EndEffect),
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
                            RelationType = ElementView.RelationType.SingleTemplate,
                            ElementType = ElementType.ReminderTemplate,
                            RelationItems =
                                x.RoundReminderTemplate != null
                                    ? new List<ElementView.Relation.ElementRelationItem>()
                                    {
                                        new ElementView.Relation.ElementRelationItem()
                                        {
                                            Id = x.RoundReminderTemplate.Id,
                                            Name = x.RoundReminderTemplate.Reminder,
                                        }
                                    }
                                    : new List<ElementView.Relation.ElementRelationItem>()
                        },
                    }
                })
                .SingleOrDefault();
        }

        public ElementList GetTemplateList()
        {
            return new ElementList()
            {
                Title = "Effect Counters",
                Type = ElementType.EffectCounterTemplate,
                ElementColumns = TemplateListModels.CounterTemplateColumns,
                Elements = _daoSearchService
                    .GetElements<CounterTemplate>()
                    .Where(x => x.Id != 1 && !(x is EffectCounterTemplate))
                    .Select(x => new TemplateListModels.CounterTemplateModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    })
                    .Cast<dynamic>()
                    .ToList()
            };
        }
    }
}
