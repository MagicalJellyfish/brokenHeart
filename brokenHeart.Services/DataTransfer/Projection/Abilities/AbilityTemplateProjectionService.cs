using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Abilities
{
    internal class AbilityTemplateProjectionService
        : IElementProjectionService,
            ITemplateProjectionService
    {
        public ElementType ProjectionType => ElementType.AbilityTemplate;

        private readonly IDaoSearchService _daoSearchService;

        public AbilityTemplateProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<AbilityTemplate> abilityTemplates =
                _daoSearchService.GetSingleElement<AbilityTemplate>(new DaoSearch() { Id = id });

            return abilityTemplates
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(AbilityTemplate.Description),
                            Title = "Description",
                            Content = x.Description
                        },
                        new ElementView.Text()
                        {
                            FieldId = nameof(AbilityTemplate.Abstract),
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
                            FieldId = nameof(AbilityTemplate.Name),
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(AbilityTemplate.TargetType),
                            Title = "Target Type",
                            Content = new ElementView.Field.EnumContent()
                            {
                                Type = ElementView.Field.EnumContent.EnumType.TargetType,
                                Value = (int)x.TargetType
                            },
                            Type = ElementView.FieldType.Enum
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(AbilityTemplate.Shortcut),
                            Title = "Shortcut",
                            Content = x.Shortcut,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(AbilityTemplate.Rolls),
                            Title = "Roll",
                            Content = x.Self,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(AbilityTemplate.Target),
                            Title = "Target's Roll/DC",
                            Content = x.Target,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(AbilityTemplate.Damage),
                            Title = "Damage",
                            Content = x.Damage,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(AbilityTemplate.Range),
                            Title = "Range",
                            Content = x.Range,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(AbilityTemplate.MaxUses),
                            Title = "Maximum Uses",
                            Content = x.MaxUses,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(AbilityTemplate.ReplenishType),
                            Title = "Replenish Type",
                            Content = new ElementView.Field.EnumContent()
                            {
                                Type = ElementView.Field.EnumContent.EnumType.ReplenishType,
                                Value = (int)x.ReplenishType
                            },
                            Type = ElementView.FieldType.Enum
                        },
                    },
                    Relations = new()
                    {
                        new ElementView.Relation()
                        {
                            Title = "Effect Templates",
                            RelationType = ElementView.RelationType.MultipleTemplates,
                            ElementType = ElementType.EffectTemplate,
                            RelationItems = x.AppliedEffectTemplates!.Select(
                                    effectTemplate => new ElementView.Relation.ElementRelationItem()
                                    {
                                        Id = effectTemplate.Id,
                                        Name = effectTemplate.Name,
                                    }
                                )
                                .ToList()
                        },
                        new ElementView.Relation()
                        {
                            Title = "Rolls",
                            RelationType = ElementView.RelationType.Roll,
                            RelationItems = x.Rolls!.Select(
                                roll => new ElementView.Relation.RollRelationItem()
                                {
                                    Id = roll.Id,
                                    Name = roll.Name,
                                    Roll = roll.Instruction
                                }
                            )
                        }
                    }
                })
                .SingleOrDefault();
        }

        public ElementList GetTemplateList()
        {
            return new ElementList()
            {
                Title = "Abilities",
                Type = ElementType.AbilityTemplate,
                ElementColumns = TemplateListModels.AbstractTemplateColumns,
                Elements = _daoSearchService
                    .GetElements<AbilityTemplate>()
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
