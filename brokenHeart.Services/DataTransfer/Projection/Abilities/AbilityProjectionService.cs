using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search.Abilities;

namespace brokenHeart.Services.DataTransfer.Projection.Abilities
{
    internal class AbilityProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Ability;

        private readonly IAbilitySearchService _abilitySearchService;

        public AbilityProjectionService(IAbilitySearchService abilitySearchService)
        {
            _abilitySearchService = abilitySearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Ability> abilities = _abilitySearchService.GetAbilities(
                new AbilitySearch() { Id = id }
            );

            if (abilities.Count() == 0 || abilities.Count() > 1)
            {
                return null;
            }

            return abilities
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text() { Title = "Description", Content = x.Description },
                        new ElementView.Text() { Title = "Abstract", Content = x.Abstract }
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
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
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
                            Title = "Shortcut",
                            Content = x.Shortcut,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            Title = "Roll",
                            Content = x.Self,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            Title = "Target's Roll/DC",
                            Content = x.Target,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            Title = "Damage",
                            Content = x.Damage,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            Title = "Range",
                            Content = x.Range,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            Title = "Uses",
                            Content = new ElementView.Field.MultiField()
                            {
                                Separator = "/",
                                Fields = new()
                                {
                                    new ElementView.Field()
                                    {
                                        Title = "Uses",
                                        Content = x.Uses,
                                        Type = ElementView.FieldType.Number
                                    },
                                    new ElementView.Field()
                                    {
                                        Title = "Maximum Uses",
                                        Content = x.MaxUses,
                                        Type = ElementView.FieldType.Number
                                    }
                                }
                            },
                            Type = ElementView.FieldType.Multi
                        },
                        new ElementView.Field()
                        {
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
                            RelationType = ElementView.RelationType.MultipleElements,
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
    }
}
