using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.DAO.Modifiers.Effects.Injuries;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Modifiers.Effects
{
    internal class EffectTemplateProjectionService
        : IElementProjectionService,
            ITemplateProjectionService
    {
        public ElementType ProjectionType => ElementType.EffectTemplate;

        private readonly IDaoSearchService _daoSearchService;

        public EffectTemplateProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<EffectTemplate> effects = _daoSearchService.GetSingleElement<EffectTemplate>(
                new DaoSearch() { Id = id }
            );

            return effects
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(EffectTemplate.Description),
                            Title = "Description",
                            Content = x.Description
                        },
                        new ElementView.Text()
                        {
                            FieldId = nameof(EffectTemplate.Abstract),
                            Title = "Abstract",
                            Content = x.Abstract
                        }
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
                            FieldId = nameof(EffectTemplate.Name),
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectTemplate.MaxHp),
                            Title = "Maximum HP Increase",
                            Content = x.MaxHp,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectTemplate.MovementSpeed),
                            Title = "Movement Speed Increase",
                            Content = x.MovementSpeed,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectTemplate.Evasion),
                            Title = "Evasion",
                            Content = x.Evasion,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectTemplate.Armor),
                            Title = "Armor",
                            Content = x.Armor,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectTemplate.Hp),
                            Title = "HP healed per round",
                            Content = x.Hp,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectTemplate.MaxTempHp),
                            Title = "Temporary HP",
                            Content = x.MaxTempHp,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(EffectTemplate.Duration),
                            Title = "Duration",
                            Content = x.Duration,
                            Type = ElementView.FieldType.String
                        },
                    },
                    Relations = new()
                    {
                        new ElementView.Relation()
                        {
                            Title = "Abilities",
                            RelationType = ElementView.RelationType.MultipleTemplates,
                            ElementType = ElementType.AbilityTemplate,
                            RelationItems = x
                                .AbilityTemplates.Select(
                                    ability => new ElementView.Relation.ElementRelationItem()
                                    {
                                        Id = ability.Id,
                                        Name = ability.Name,
                                    }
                                )
                                .ToList()
                        },
                        new ElementView.Relation()
                        {
                            Title = "Counters",
                            RelationType = ElementView.RelationType.MultipleTemplates,
                            ElementType = ElementType.CounterTemplate,
                            RelationItems = x
                                .CounterTemplates.Select(
                                    counter => new ElementView.Relation.ElementRelationItem()
                                    {
                                        Id = counter.Id,
                                        Name = counter.Name,
                                    }
                                )
                                .ToList()
                        },
                        new ElementView.Relation()
                        {
                            Title = "Effect-Counter",
                            RelationType = ElementView.RelationType.SingleTemplate,
                            ElementType = ElementType.EffectCounterTemplate,
                            RelationItems =
                                x.EffectCounterTemplate != null
                                    ? new List<ElementView.Relation.ElementRelationItem>()
                                    {
                                        new ElementView.Relation.ElementRelationItem()
                                        {
                                            Id = x.EffectCounterTemplate.Id,
                                            Name = x.EffectCounterTemplate.Name,
                                        }
                                    }
                                    : new List<ElementView.Relation.ElementRelationItem>()
                        },
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
                        new ElementView.Relation()
                        {
                            Title = "Stats",
                            RelationType = ElementView.RelationType.Stats,
                            RelationItems = x.StatIncreases.Select(
                                statIncrease => new ElementView.Relation.StatRelationItem()
                                {
                                    Id = statIncrease.Id,
                                    StatId = statIncrease.Stat.Id,
                                    Name = statIncrease.Stat.Name,
                                    Value = statIncrease.Value
                                }
                            )
                        },
                    }
                })
                .SingleOrDefault();
        }

        public ElementList GetTemplateList()
        {
            return new ElementList()
            {
                Title = "Effects",
                Type = ElementType.EffectTemplate,
                ElementColumns = TemplateListModels.AbstractTemplateColumns,
                Elements = _daoSearchService
                    .GetElements<EffectTemplate>()
                    .Where(x => !(x is InjuryEffectTemplate))
                    .Select(x => new TemplateListModels.AbstractTemplateModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Abstract = x.Abstract
                    })
                    .Cast<dynamic>()
                    .ToList()
            };
        }
    }
}
