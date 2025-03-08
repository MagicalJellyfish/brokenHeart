using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search.Modifiers;
using brokenHeart.Services.DataTransfer.Search.Abilities;

namespace brokenHeart.Services.DataTransfer.Projection.Abilities
{
    internal class TraitProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Trait;

        private readonly ITraitSearchService _traitSearchService;

        public TraitProjectionService(ITraitSearchService traitSearchService)
        {
            _traitSearchService = traitSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Trait> traits = _traitSearchService.GetTraits(new TraitSearch() { Id = id });

            if (traits.Count() == 0 || traits.Count() > 1)
            {
                return null;
            }

            return traits
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
                            Title = "Maximum HP Increase",
                            Content = x.MaxHp,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            Title = "Movement Speed Increase",
                            Content = x.MovementSpeed,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            Title = "Evasion",
                            Content = x.Evasion,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            Title = "Armor",
                            Content = x.Armor,
                            Type = ElementView.FieldType.Number
                        },
                        new ElementView.Field()
                        {
                            Title = "Active",
                            Content = x.Active,
                            Type = ElementView.FieldType.Boolean
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
                                    : null
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
