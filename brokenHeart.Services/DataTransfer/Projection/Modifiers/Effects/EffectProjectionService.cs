using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Modifiers;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Modifiers.Effects;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Abilities
{
    internal class EffectProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Effect;

        private readonly IDaoSearchService _daoSearchService;

        public EffectProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Effect> effects = _daoSearchService.GetSingleElement<Effect>(
                new DaoSearch() { Id = id }
            );

            return effects
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = (int)ModifierField.Description,
                            Title = "Description",
                            Content = x.Description
                        },
                        new ElementView.Text()
                        {
                            FieldId = (int)ModifierField.Abstract,
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
                            FieldId = (int)ModifierField.Name,
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)ModifierField.MaxHp,
                            Title = "Maximum HP Increase",
                            Content = x.MaxHp,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)ModifierField.MovementSpeed,
                            Title = "Movement Speed Increase",
                            Content = x.MovementSpeed,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)ModifierField.Evasion,
                            Title = "Evasion",
                            Content = x.Evasion,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)ModifierField.Armor,
                            Title = "Armor",
                            Content = x.Armor,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)EffectField.Hp,
                            Title = "HP healed per round",
                            Content = x.Hp,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)EffectField.MaxTempHp,
                            Title = "Temporary HP",
                            Content = x.MaxTempHp,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)EffectField.Duration,
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
                            RelationType = ElementView.RelationType.MultipleElements,
                            ElementType = ElementType.Ability,
                            RelationItems = x.Abilities!.Select(
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
                            RelationType = ElementView.RelationType.MultipleElements,
                            ElementType = ElementType.Counter,
                            RelationItems = x.Counters!.Select(
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
                            RelationType = ElementView.RelationType.SingleElement,
                            ElementType = ElementType.EffectCounter,
                            RelationItems =
                                x.EffectCounter != null
                                    ? new List<ElementView.Relation.ElementRelationItem>()
                                    {
                                        new ElementView.Relation.ElementRelationItem()
                                        {
                                            Id = x.EffectCounter.Id,
                                            Name = x.EffectCounter.Name,
                                        }
                                    }
                                    : new List<ElementView.Relation.ElementRelationItem>()
                        },
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
    }
}
