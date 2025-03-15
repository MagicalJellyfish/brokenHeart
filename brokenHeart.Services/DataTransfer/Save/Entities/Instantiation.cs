using System.Linq.Expressions;
using brokenHeart.Database.DAO.Abilities;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.DAO.Modifiers.Effects.Injuries;
using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;
using LinqKit;

namespace brokenHeart.Services.DataTransfer.Save.Entities
{
    internal static class Instantiation
    {
        public static Expression<Func<AbilityTemplate, Ability>> InstantiateAbility = (ability) =>
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
                Uses = ability.MaxUses,
                MaxUses = ability.MaxUses,
                ReplenishType = ability.ReplenishType,
                Rolls = ability.Rolls.Select(x => InstantiateRoll.Invoke(x)).ToList(),
                AppliedEffectTemplates = ability.AppliedEffectTemplates
            };

        public static Expression<Func<TraitTemplate, Trait>> InstantiateTrait = (trait) =>
            new Trait()
            {
                Name = trait.Name,
                Abstract = trait.Abstract,
                Description = trait.Description,
                MaxHp = trait.MaxHp,
                MovementSpeed = trait.MovementSpeed,
                Armor = trait.Armor,
                Evasion = trait.Evasion,
                Active = true,
                Abilities = trait
                    .AbilityTemplates.Select(x => InstantiateAbility.Invoke(x))
                    .ToList(),
                Counters = trait
                    .CounterTemplates.Select(x => InstantiateCounter.Invoke(x))
                    .ToList(),

                RoundReminder = InstantiateRoundReminder.Invoke(trait.RoundReminderTemplate),
                StatIncreases = trait
                    .StatIncreases.Select(x => InstantiateStatValue.Invoke(x))
                    .ToList()
            };

        public static Expression<Func<ItemTemplate, Item>> InstantiateItem = (item) =>
            new Item()
            {
                Name = item.Name,
                Abstract = item.Abstract,
                Description = item.Description,
                MaxHp = item.MaxHp,
                MovementSpeed = item.MovementSpeed,
                Armor = item.Armor,
                Evasion = item.Evasion,
                Equipped = true,
                Amount = item.Amount,
                Unit = item.Unit,
                Abilities = item
                    .AbilityTemplates.Select(x => InstantiateAbility.Invoke(x))
                    .ToList(),
                Counters = item.CounterTemplates.Select(x => InstantiateCounter.Invoke(x)).ToList(),

                RoundReminder = InstantiateRoundReminder.Invoke(item.RoundReminderTemplate),
                StatIncreases = item
                    .StatIncreases.Select(x => InstantiateStatValue.Invoke(x))
                    .ToList()
            };

        public static Expression<Func<EffectTemplate, Effect>> InstantiateEffect = (effect) =>
            new Effect()
            {
                Name = effect.Name,
                Abstract = effect.Abstract,
                Description = effect.Description,
                MaxHp = effect.MaxHp,
                MovementSpeed = effect.MovementSpeed,
                Armor = effect.Armor,
                Evasion = effect.Evasion,
                Hp = effect.Hp,
                MaxTempHp = effect.MaxTempHp,
                Duration = effect.Duration,
                EffectCounter = InstantiateEffectCounter.Invoke(effect.EffectCounterTemplate),
                Abilities = effect
                    .AbilityTemplates.Select(x => InstantiateAbility.Invoke(x))
                    .ToList(),
                Counters = effect
                    .CounterTemplates.Select(x => InstantiateCounter.Invoke(x))
                    .ToList(),

                RoundReminder = InstantiateRoundReminder.Invoke(effect.RoundReminderTemplate),
                StatIncreases = effect
                    .StatIncreases.Select(x => InstantiateStatValue.Invoke(x))
                    .ToList()
            };

        public static Expression<Func<InjuryEffectTemplate, InjuryEffect>> InstantiateInjuryEffect =
            (injuryEffect) =>
                new InjuryEffect()
                {
                    Name = injuryEffect.Name,
                    Abstract = injuryEffect.Abstract,
                    Description = injuryEffect.Description,
                    MaxHp = injuryEffect.MaxHp,
                    MovementSpeed = injuryEffect.MovementSpeed,
                    Armor = injuryEffect.Armor,
                    Evasion = injuryEffect.Evasion,
                    Hp = injuryEffect.Hp,
                    MaxTempHp = injuryEffect.MaxTempHp,
                    Duration = injuryEffect.Duration,
                    Bodypart = injuryEffect.Bodypart,
                    InjuryLevel = injuryEffect.InjuryLevel,
                    EffectCounter = InstantiateEffectCounter.Invoke(
                        injuryEffect.EffectCounterTemplate
                    ),
                    Abilities = injuryEffect
                        .AbilityTemplates.Select(x => InstantiateAbility.Invoke(x))
                        .ToList(),
                    Counters = injuryEffect
                        .CounterTemplates.Select(x => InstantiateCounter.Invoke(x))
                        .ToList(),

                    RoundReminder = InstantiateRoundReminder.Invoke(
                        injuryEffect.RoundReminderTemplate
                    ),
                    StatIncreases = injuryEffect
                        .StatIncreases.Select(x => InstantiateStatValue.Invoke(x))
                        .ToList()
                };

        public static Expression<Func<CounterTemplate, Counter>> InstantiateCounter = (counter) =>
            new Counter()
            {
                Name = counter.Name,
                Description = counter.Description,
                Value = 0,
                Max = counter.Max,
                RoundBased = counter.RoundBased,
                RoundReminder = InstantiateRoundReminder.Invoke(counter.RoundReminderTemplate)
            };

        public static Expression<
            Func<EffectCounterTemplate?, EffectCounter?>
        > InstantiateEffectCounter = (effectCounter) =>
            effectCounter == null
                ? null
                : new EffectCounter()
                {
                    Name = effectCounter.Name,
                    Description = effectCounter.Description,
                    Value = 0,
                    Max = effectCounter.Max,
                    RoundBased = effectCounter.RoundBased,
                    EndEffect = effectCounter.EndEffect,
                    RoundReminder = InstantiateRoundReminder.Invoke(
                        effectCounter.RoundReminderTemplate
                    )
                };

        public static Expression<
            Func<RoundReminderTemplate?, RoundReminder?>
        > InstantiateRoundReminder = (roundReminder) =>
            roundReminder == null
                ? null
                : new RoundReminder()
                {
                    Reminder = roundReminder.Reminder,
                    Reminding = roundReminder.Reminding,
                };

        public static Expression<Func<StatValue, StatValue>> InstantiateStatValue = (statValue) =>
            new StatValue() { StatId = statValue.StatId, Value = statValue.Value };

        public static Expression<Func<Roll, Roll>> InstantiateRoll = (roll) =>
            new Roll() { Name = roll.Name, Instruction = roll.Instruction };
    }
}
