using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;
using LinqKit;
using System.Linq.Expressions;

namespace brokenHeart.Services.DataTransfer.Projection.Characters
{
    internal class CharacterProjectionService : ICharacterProjectionService
    {
        private readonly ICharacterSearchService _characterSearchService;

        public CharacterProjectionService(ICharacterSearchService characterSearchService)
        {
            _characterSearchService = characterSearchService;
        }

        public List<SimpleCharacter> GetSimpleCharacters(CharacterSearch search)
        {
            IQueryable<Character> characters = _characterSearchService.GetCharacters(search);

            return characters.Select(SimpleCharacter.Map).ToList();
        }

        public CharacterView? GetCharacterView(CharacterSearch search)
        {
            IQueryable<Character> characters = _characterSearchService.GetSingleCharacter(search);

            List<CharacterView.AbilityModel> abilities = characters
                .SelectMany(x =>
                    x.Abilities.Select(ability => AbilityProjection.Invoke(ability, "Character"))
                )
                .ToList()
                .Concat(
                    characters.SelectMany(x =>
                        x.Traits.SelectMany(trait =>
                            trait
                                .Abilities.AsQueryable()
                                .Select(ability =>
                                    AbilityProjection.Invoke(
                                        ability,
                                        $"Trait {ability.Modifier.Name}"
                                    )
                                )
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Items.SelectMany(item =>
                            item.Abilities.AsQueryable()
                                .Select(ability =>
                                    AbilityProjection.Invoke(
                                        ability,
                                        $"Item {ability.Modifier.Name}"
                                    )
                                )
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Effects.SelectMany(effect =>
                            effect
                                .Abilities.AsQueryable()
                                .Select(ability =>
                                    AbilityProjection.Invoke(
                                        ability,
                                        $"Effect {ability.Modifier.Name}"
                                    )
                                )
                        )
                    )
                )
                .OrderBy(x => x.ViewPosition)
                .ToList();

            List<CharacterView.CounterModel> counters = characters
                .SelectMany(x =>
                    x.Counters.AsQueryable()
                        .Select(counter => CounterProjection.Invoke(counter, "Character"))
                )
                .ToList()
                .Concat(
                    characters.SelectMany(x =>
                        x.Traits.SelectMany(trait =>
                            trait.Counters.Select(counter =>
                                CounterProjection.Invoke(counter, $"Trait {counter.Modifier.Name}")
                            )
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Items.SelectMany(item =>
                            item.Counters.Select(counter =>
                                CounterProjection.Invoke(counter, $"Item {counter.Modifier.Name}")
                            )
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Effects.SelectMany(effect =>
                            effect.Counters.Select(counter =>
                                CounterProjection.Invoke(counter, $"Effect {counter.Modifier.Name}")
                            )
                        )
                    )
                )
                .OrderBy(y => y.ViewPosition)
                .ToList();

            List<CharacterView.ReminderModel> reminders = characters
                .SelectMany(x =>
                    x.RoundReminders.AsQueryable()
                        .Select(reminder => ReminderProjection.Invoke(reminder, "Character"))
                )
                .ToList()
                .Concat(
                    characters.SelectMany(x =>
                        x.Counters.Select(counter =>
                            ReminderProjection.Invoke(
                                counter.RoundReminder,
                                $"Counter {counter.Name}"
                            )
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Traits.SelectMany(trait =>
                            trait.Counters.Select(counter =>
                                ReminderProjection.Invoke(
                                    counter.RoundReminder,
                                    $"Counter {counter.Name} of Trait {counter.Modifier.Name}"
                                )
                            )
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Traits.Select(trait =>
                            ReminderProjection.Invoke(trait.RoundReminder, $"Trait {trait.Name}")
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Items.SelectMany(item =>
                            item.Counters.Select(counter =>
                                ReminderProjection.Invoke(
                                    counter.RoundReminder,
                                    $"Counter {counter.Name} of Item {counter.Modifier.Name}"
                                )
                            )
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Items.Select(item =>
                            ReminderProjection.Invoke(item.RoundReminder, $"Item {item.Name}")
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Effects.SelectMany(effect =>
                            effect.Counters.Select(counter =>
                                ReminderProjection.Invoke(
                                    counter.RoundReminder,
                                    $"Counter {counter.Name} of Effect {counter.Modifier.Name}"
                                )
                            )
                        )
                    )
                )
                .Concat(
                    characters.SelectMany(x =>
                        x.Effects.Select(effect =>
                            ReminderProjection.Invoke(effect.RoundReminder, $"Effect {effect.Name}")
                        )
                    )
                )
                .Where(x => x != null)
                .OrderBy(x => x.ViewPosition)
                .ToList()!;

            return characters
                .Select(x => new CharacterView()
                {
                    Id = x.Id,
                    Name = x.Name,

                    IsNPC = x.IsNPC,
                    DefaultShortcut = x.DefaultShortcut,

                    Age = x.Age,
                    Height = x.Height,
                    Weight = x.Weight,

                    MovementSpeed = x.MovementSpeed,
                    Evasion = x.Evasion,
                    Armor = x.Armor,
                    Defense = x.Evasion + x.Armor,

                    Experience = x.Experience,
                    Description = x.Description,
                    Notes = x.Notes,

                    Money = x.Money,
                    C = x.C,

                    Hp = x.Hp,
                    MaxHp = x.MaxHp,
                    TempHp = x.TempHp,
                    MaxTempHp = x.MaxTempHp,

                    DeathCounter = new CharacterView.DeathCounterModel()
                    {
                        Id = x.DeathCounter.Id,
                        ValueFieldId = nameof(Counter.Value),
                        Max = x.DeathCounter.Max,
                        Value = x.DeathCounter.Value
                    },

                    Stats = x
                        .Stats.Select(stat => new CharacterView.StatModel()
                        {
                            StatId = stat.StatId,
                            Name = stat.Stat.Name,
                            Value = stat.Value
                        })
                        .ToList(),

                    HpImpacts = x
                        .Effects.Where(effect => effect.Hp != "")
                        .Select(effect => new CharacterView.HpImpactModel()
                        {
                            Name = effect.Name,
                            Value = effect.Hp
                        })
                        .ToList(),

                    Injuries = x
                        .BodypartConditions.Select(
                            bodypartCondition => new CharacterView.InjuryModel()
                            {
                                Bodypart = bodypartCondition.Bodypart.Id,
                                InjuryLevel = bodypartCondition.InjuryLevel
                            }
                        )
                        .ToList(),

                    ElementLists = new()
                    {
                        new ElementList()
                        {
                            Title = "Abilities",
                            Type = ElementType.Ability,
                            ElementColumns = AbilityColumns,
                            Elements = abilities.Cast<dynamic>().ToList()
                        },
                        new ElementList()
                        {
                            Title = "Traits",
                            Type = ElementType.Trait,
                            ElementColumns = TraitColumns,
                            Elements = x
                                .Traits.Select(trait => new CharacterView.TraitModel()
                                {
                                    Id = trait.Id,
                                    ViewPosition = trait.ViewPosition,
                                    Name = trait.Name,
                                    Abstract = trait.Abstract,
                                    Active = trait.Active
                                })
                                .OrderBy(y => y.ViewPosition)
                                .Cast<dynamic>()
                                .ToList()
                        },
                        new ElementList()
                        {
                            Title = "Items",
                            Type = ElementType.Item,
                            ElementColumns = ItemColumns,
                            Elements = x
                                .Items.Select(item => new CharacterView.ItemModel()
                                {
                                    Id = item.Id,
                                    ViewPosition = item.ViewPosition,
                                    Name = item.Name,
                                    Abstract = item.Abstract,
                                    Equipped = item.Equipped,
                                    Amount = item.Amount
                                })
                                .OrderBy(y => y.ViewPosition)
                                .Cast<dynamic>()
                                .ToList()
                        },
                        new ElementList()
                        {
                            Title = "Effects",
                            SubTabs = true,
                            Elements = new()
                            {
                                new ElementList()
                                {
                                    Title = "Effects",
                                    Type = ElementType.Effect,
                                    ElementColumns = EffectColumns,
                                    Elements = x
                                        .Effects.Select(x => new CharacterView.EffectModel()
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            ViewPosition = x.ViewPosition,
                                            Abstract = x.Abstract
                                        })
                                        .OrderBy(y => y.ViewPosition)
                                        .Cast<dynamic>()
                                        .ToList()
                                },
                                new ElementList()
                                {
                                    Title = "Injury Effect Templates",
                                    Type = ElementType.InjuryEffect,
                                    ElementColumns = EffectColumns,
                                    Elements = x
                                        .InjuryEffects.Select(x => new CharacterView.EffectModel()
                                        {
                                            Id = x.Id,
                                            ViewPosition = x.ViewPosition,
                                            Name = x.Name,
                                            Abstract = x.Abstract
                                        })
                                        .OrderBy(y => y.ViewPosition)
                                        .Cast<dynamic>()
                                        .ToList()
                                },
                            }
                        },
                        new ElementList()
                        {
                            Title = "Counters",
                            Type = ElementType.Counter,
                            ElementColumns = CounterColumns,
                            Elements = counters.Cast<dynamic>().ToList()
                        },
                        new ElementList()
                        {
                            Title = "Reminders",
                            Type = ElementType.Reminder,
                            ElementColumns = RoundReminderColumns,
                            Elements = reminders.Cast<dynamic>().ToList()
                        },
                        new ElementList()
                        {
                            Title = "Variables",
                            Type = ElementType.Variable,
                            ElementColumns = VariableColumns,
                            Elements = x
                                .Variables.Select(x => new CharacterView.VariableModel()
                                {
                                    Id = x.Id,
                                    ViewPosition = x.ViewPosition,
                                    Name = x.Name,
                                    Value = x.Value
                                })
                                .OrderBy(y => y.ViewPosition)
                                .Cast<dynamic>()
                                .ToList()
                        },
                    },
                })
                .SingleOrDefault();
        }

        public static List<ElementList.ElementColumn> AbilityColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "Name",
                    Property = "name",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Abstract",
                    Property = "abstract",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Uses",
                    FieldId = nameof(Ability.Uses),
                    Property = "uses",
                    PropertyOf = "maxUses",
                    ColumnType = ElementList.ElementColumnType.InputOf,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Source",
                    Property = "source",
                    ColumnType = ElementList.ElementColumnType.Text,
                }
            };

        public static List<ElementList.ElementColumn> TraitColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "Name",
                    Property = "name",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Abstract",
                    Property = "abstract",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Active",
                    FieldId = nameof(Trait.Active),
                    Property = "active",
                    ColumnType = ElementList.ElementColumnType.Checkbox,
                }
            };

        public static List<ElementList.ElementColumn> ItemColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "Name",
                    Property = "name",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Abstract",
                    Property = "abstract",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Equipped",
                    FieldId = nameof(Item.Equipped),
                    Property = "equipped",
                    ColumnType = ElementList.ElementColumnType.Checkbox,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Amount",
                    FieldId = nameof(Item.Amount),
                    Property = "amount",
                    ColumnType = ElementList.ElementColumnType.Input,
                }
            };

        public static List<ElementList.ElementColumn> EffectColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "Name",
                    Property = "name",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Abstract",
                    Property = "abstract",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                }
            };

        public static List<ElementList.ElementColumn> CounterColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "Name",
                    Property = "name",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Description",
                    Property = "description",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Value",
                    FieldId = nameof(Counter.Value),
                    Property = "value",
                    PropertyOf = "max",
                    ColumnType = ElementList.ElementColumnType.InputOf,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Source",
                    Property = "source",
                    ColumnType = ElementList.ElementColumnType.Text,
                }
            };

        public static List<ElementList.ElementColumn> RoundReminderColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "Reminder",
                    Property = "reminder",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Reminding",
                    FieldId = nameof(RoundReminder.Reminding),
                    Property = "reminding",
                    ColumnType = ElementList.ElementColumnType.Checkbox,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Source",
                    Property = "source",
                    ColumnType = ElementList.ElementColumnType.Text,
                },
            };

        public static List<ElementList.ElementColumn> VariableColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "Name",
                    Property = "name",
                    ColumnType = ElementList.ElementColumnType.Text,
                    Searchable = true,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Value",
                    Property = "value",
                    ColumnType = ElementList.ElementColumnType.Text,
                },
            };

        public Expression<Func<Ability, string, CharacterView.AbilityModel>> AbilityProjection = (
            ability,
            source
        ) =>
            new CharacterView.AbilityModel()
            {
                Id = ability.Id,
                ViewPosition = ability.ViewPosition,
                Name = ability.Name,
                Abstract = ability.Abstract,
                Uses = ability.Uses,
                MaxUses = ability.MaxUses,
                Source = source
            };

        public Expression<Func<Counter, string, CharacterView.CounterModel>> CounterProjection = (
            counter,
            source
        ) =>
            new CharacterView.CounterModel()
            {
                Id = counter.Id,
                ViewPosition = counter.ViewPosition,
                Name = counter.Name,
                Description = counter.Description,
                Value = counter.Value,
                Max = counter.Max,
                Source = source
            };

        public Expression<
            Func<RoundReminder?, string, CharacterView.ReminderModel?>
        > ReminderProjection = (reminder, source) =>
            reminder == null
                ? null
                : new CharacterView.ReminderModel()
                {
                    Id = reminder.Id,
                    ViewPosition = reminder.ViewPosition,
                    Reminder = reminder.Reminder,
                    Reminding = reminder.Reminding,
                    Source = source
                };
    }
}
