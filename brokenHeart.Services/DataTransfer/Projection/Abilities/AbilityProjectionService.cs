using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection.Abilities
{
    internal class AbilityProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Ability;

        private readonly BrokenDbContext _context;

        public AbilityProjectionService(BrokenDbContext context)
        {
            _context = context;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Ability> abilities = _context.Abilities.Where(x => x.Id == id);

            return abilities
                .Select(x => new ElementView()
                {
                    Texts = new()
                    {
                        new ElementView.Text()
                        {
                            FieldId = nameof(Ability.Description),
                            Title = "Description",
                            Content = x.Description,
                        },
                        new ElementView.Text()
                        {
                            FieldId = nameof(Ability.Abstract),
                            Title = "Abstract",
                            Content = x.Abstract,
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
                            FieldId = nameof(Ability.Name),
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Ability.TargetType),
                            Title = "Target Type",
                            Content = new ElementView.Field.EnumContent()
                            {
                                Type = ElementView.Field.EnumContent.EnumType.TargetType,
                                Value = (int)x.TargetType,
                            },
                            Type = ElementView.FieldType.Enum,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Ability.Shortcut),
                            Title = "Shortcut",
                            Content = x.Shortcut,
                            Type = ElementView.FieldType.String,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Ability.Self),
                            Title = "Roll",
                            Content = x.Self,
                            Type = ElementView.FieldType.String,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Ability.Target),
                            Title = "Target's Roll/DC",
                            Content = x.Target,
                            Type = ElementView.FieldType.String,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Ability.Damage),
                            Title = "Damage",
                            Content = x.Damage,
                            Type = ElementView.FieldType.String,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Ability.Range),
                            Title = "Range",
                            Content = x.Range,
                            Type = ElementView.FieldType.String,
                        },
                        new ElementView.Field()
                        {
                            Title = "Uses",
                            Type = ElementView.FieldType.Multi,
                            Content = new ElementView.Field.MultiField()
                            {
                                Separator = "/",
                                Fields = new()
                                {
                                    new ElementView.Field()
                                    {
                                        FieldId = nameof(Ability.Uses),
                                        Title = "Uses",
                                        Content = x.Uses,
                                        Type = ElementView.FieldType.Number,
                                    },
                                    new ElementView.Field()
                                    {
                                        FieldId = nameof(Ability.MaxUses),
                                        Title = "Max Uses",
                                        Content = x.MaxUses,
                                        Type = ElementView.FieldType.Number,
                                    },
                                },
                            },
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Ability.ReplenishType),
                            Title = "Replenish Type",
                            Content = new ElementView.Field.EnumContent()
                            {
                                Type = ElementView.Field.EnumContent.EnumType.ReplenishType,
                                Value = (int)x.ReplenishType,
                            },
                            Type = ElementView.FieldType.Enum,
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
                                .ToList(),
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
                                    Roll = roll.Instruction,
                                }
                            ),
                        },
                    },
                })
                .SingleOrDefault();
        }
    }
}
