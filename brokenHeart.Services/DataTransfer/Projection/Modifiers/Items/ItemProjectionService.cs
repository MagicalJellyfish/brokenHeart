using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search.Modifiers;
using brokenHeart.Services.DataTransfer.Search.Abilities;

namespace brokenHeart.Services.DataTransfer.Projection.Abilities
{
    internal class ItemProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Item;

        private readonly IItemSearchService _itemSearchService;

        public ItemProjectionService(IItemSearchService itemSearchService)
        {
            _itemSearchService = itemSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Item> items = _itemSearchService.GetItems(new ItemSearch() { Id = id });

            if (items.Count() == 0 || items.Count() > 1)
            {
                return null;
            }

            return items
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
                            Title = "Equipped",
                            Content = x.Equipped,
                            Type = ElementView.FieldType.Boolean
                        },
                        new ElementView.Field()
                        {
                            Title = "Amount",
                            Type = ElementView.FieldType.Multi,
                            Content = new ElementView.Field.MultiField()
                            {
                                Separator = "",
                                Fields = new()
                                {
                                    new ElementView.Field()
                                    {
                                        Title = "Amount",
                                        Content = x.Amount,
                                        Type = ElementView.FieldType.Number
                                    },
                                    new ElementView.Field()
                                    {
                                        Title = "Unit",
                                        Content = x.Unit,
                                        Type = ElementView.FieldType.String
                                    }
                                }
                            },
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
