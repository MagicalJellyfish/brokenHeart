using System.Linq.Expressions;
using brokenHeart.Database.DAO.Abilities;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;
using LinqKit;

namespace brokenHeart.Services.DataTransfer.Save.Entities
{
    internal static class Duplication
    {
        public static Expression<Func<Item, Item>> DuplicateItem = (item) =>
            new Item()
            {
                Name = item.Name,
                Abstract = item.Abstract,
                Description = item.Description,
                MaxHp = item.MaxHp,
                MovementSpeed = item.MovementSpeed,
                Armor = item.Armor,
                Evasion = item.Evasion,
                Equipped = item.Equipped,
                Amount = item.Amount,
                Unit = item.Unit,
                ViewPosition = item.ViewPosition,
                Abilities = item.Abilities.Select(x => DuplicateAbility.Invoke(x)).ToList(),
                Counters = item.Counters.Select(x => DuplicateCounter.Invoke(x)).ToList(),

                RoundReminder = DuplicateRoundReminder.Invoke(item.RoundReminder),
                StatIncreases = item
                    .StatIncreases.Select(x => DuplicateStatValue.Invoke(x))
                    .ToList(),
            };

        public static Expression<Func<Ability, Ability>> DuplicateAbility = (ability) =>
            new Ability()
            {
                Name = ability.Name,
                Abstract = ability.Abstract,
                Description = ability.Description,
                Shortcut = ability.Shortcut,
                TargetType = ability.TargetType,
                CanInjure = ability.CanInjure,
                Self = ability.Self,
                Target = ability.Target,
                Damage = ability.Damage,
                Range = ability.Range,
                Uses = ability.Uses,
                MaxUses = ability.MaxUses,
                ReplenishType = ability.ReplenishType,
                ViewPosition = ability.ViewPosition,
                Rolls = ability.Rolls.Select(x => DuplicateRoll.Invoke(x)).ToList(),
                AppliedEffectTemplates = ability.AppliedEffectTemplates,
            };

        public static Expression<Func<Counter, Counter>> DuplicateCounter = (counter) =>
            new Counter()
            {
                Name = counter.Name,
                Description = counter.Description,
                Value = counter.Value,
                Max = counter.Max,
                RoundBased = counter.RoundBased,
                ViewPosition = counter.ViewPosition,
                RoundReminder = DuplicateRoundReminder.Invoke(counter.RoundReminder),
            };

        public static Expression<Func<RoundReminder?, RoundReminder?>> DuplicateRoundReminder = (
            roundReminder
        ) =>
            roundReminder == null
                ? null
                : new RoundReminder()
                {
                    Reminder = roundReminder.Reminder,
                    Reminding = roundReminder.Reminding,
                    ViewPosition = roundReminder.ViewPosition,
                };

        public static Expression<Func<StatValue, StatValue>> DuplicateStatValue = (statValue) =>
            new StatValue() { StatId = statValue.StatId, Value = statValue.Value };

        public static Expression<Func<Roll, Roll>> DuplicateRoll = (roll) =>
            new Roll() { Name = roll.Name, Instruction = roll.Instruction };
    }
}
