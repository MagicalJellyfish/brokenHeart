using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Combat;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Models.brokenHand;
using brokenHeart.Models.Rolling;
using brokenHeart.Services.Rolling;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.Logic.CombatTracking
{
    internal class TurnAdvancementService : ITurnAdvancementService
    {
        private readonly BrokenDbContext _context;
        private readonly ICharacterRollService _charRollService;

        public TurnAdvancementService(
            BrokenDbContext context,
            ICharacterRollService charRollService
        )
        {
            _context = context;
            _charRollService = charRollService;
        }

        public List<Message> AdvanceTurn(Combat combat, List<CombatEntry> orderedEntries)
        {
            List<Message> turnMessages = new List<Message>();
            if (combat.CurrentTurn != orderedEntries.Count - 1)
            {
                combat.CurrentTurn += 1;
            }
            else
            {
                combat.CurrentTurn = -1;
                combat.Round += 1;

                turnMessages.Add(
                    new Message("New Round!", "Current round is " + combat.Round + "!")
                );
                return turnMessages;
            }

            CombatEntry entry = orderedEntries.ElementAt(combat.CurrentTurn);
            if (entry.EventId != null)
            {
                Event @event = _context.Events.Single(x => x.Id == entry.EventId);

                if (@event.Round == combat.Round)
                {
                    turnMessages.Add(
                        new Message(
                            "Event " + @event.Name + " is happening!",
                            "Round: " + combat.Round
                        )
                    );
                }
                else
                {
                    if (!@event.Secret && @event.Round > combat.Round)
                    {
                        turnMessages.Add(
                            new Message(
                                "Event "
                                    + @event.Name
                                    + " is advancing! It triggers in "
                                    + (@event.Round - combat.Round).ToString()
                                    + " rounds!",
                                "Round: " + combat.Round
                            )
                        );
                    }
                    turnMessages.AddRange(AdvanceTurn(combat, orderedEntries));
                }

                return turnMessages;
            }

            if (entry.CharacterId != null)
            {
                string round = "Round: " + combat.Round + "\n";
                string reminderStrings = "Reminders: \n";
                // csharpier-ignore
                Character character = _context.Characters
                    .Include(x => x.Abilities)

                    .Include(x => x.RoundReminders)

                    .Include(x => x.Counters)
                    .ThenInclude(x => x.RoundReminder)

                    .Include(x => x.Items).ThenInclude(x => x.RoundReminder)
                    .Include(x => x.Items).ThenInclude(x => x.Abilities)
                    .Include(x => x.Items).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)

                    .Include(x => x.Effects).ThenInclude(x => x.RoundReminder)
                    .Include(x => x.Effects).ThenInclude(x => x.Abilities)
                    .Include(x => x.Effects).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                    .Include(x => x.Effects).ThenInclude(x => x.EffectCounter).ThenInclude(x => x.RoundReminder)

                    .Include(x => x.Traits).ThenInclude(x => x.RoundReminder)
                    .Include(x => x.Traits).ThenInclude(x => x.Abilities)
                    .Include(x => x.Traits).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                    .Single(x => x.Id == entry.CharacterId);

                string title = "It is " + character.Name + "'s turn! \n";

                List<Ability> abilities = character
                    .Abilities.Concat(character.Items.SelectMany(x => x.Abilities))
                    .Concat(character.Effects.SelectMany(x => x.Abilities))
                    .Concat(character.Traits.SelectMany(x => x.Abilities))
                    .ToList();
                foreach (Ability ability in abilities)
                {
                    if (ability.ReplenishType == ReplenishType.CombatRound)
                    {
                        ability.Uses = ability.MaxUses;
                    }
                }

                string counterStrings = "";
                List<Counter> counters = character
                    .Counters.Concat(character.Items.SelectMany(x => x.Counters))
                    .Concat(character.Traits.SelectMany(x => x.Counters))
                    .Concat(character.Effects.SelectMany(x => x.Counters))
                    .Concat(
                        character
                            .Effects.Where(x => x.EffectCounterId != null)
                            .Select(x => x.EffectCounter!)
                    )
                    .ToList();
                foreach (Counter counter in counters)
                {
                    if (counter.RoundBased)
                    {
                        counter.Value += 1;
                        if (counter.Value == counter.Max)
                        {
                            counterStrings +=
                                $"Your counter \"{counter.Name}\" has reached the maximum!\n";
                        }
                    }
                }

                string effectStrings = "";
                foreach (Effect effect in character.Effects)
                {
                    if (!string.IsNullOrEmpty(effect.Hp))
                    {
                        RollResult result = _charRollService.CharRollString(
                            effect.Hp,
                            character.Id
                        );
                        effectStrings +=
                            $"Your HP is changed by ({result.Detail}) {result.Result} from Effect \"{effect.Name}\"!\n";
                        character.Hp += result.Result;
                    }

                    if (effect.EffectCounter != null)
                    {
                        if (effect.EffectCounter.Value >= effect.EffectCounter.Max)
                        {
                            effectStrings += $"Your effect {effect.Name} has worn off!\n";
                            character.Effects.Remove(effect);
                        }
                    }
                }

                List<RoundReminder?> reminders = new List<RoundReminder?>();

                reminders.AddRange(character.RoundReminders);
                reminders.AddRange(character.Counters.Select(x => x.RoundReminder));

                reminders.AddRange(character.Items.Select(x => x.RoundReminder));
                reminders.AddRange(
                    character.Items.SelectMany(x => x.Counters.Select(x => x.RoundReminder))
                );

                reminders.AddRange(character.Effects.Select(x => x.RoundReminder));
                reminders.AddRange(
                    character.Effects.SelectMany(x => x.Counters.Select(x => x.RoundReminder))
                );
                reminders.AddRange(
                    character
                        .Effects.Where(x => x.EffectCounter != null)
                        .Select(x => x.EffectCounter!.RoundReminder!)
                );

                reminders.AddRange(character.Traits.Select(x => x.RoundReminder));
                reminders.AddRange(
                    character.Traits.SelectMany(x => x.Counters.Select(x => x.RoundReminder))
                );
                reminders = reminders.Where(x => x != null).ToList();

                if (reminders.Count == 0)
                {
                    turnMessages.Add(new Message(title, round));
                    return turnMessages;
                }

                foreach (var reminder in reminders)
                {
                    if (reminder != null)
                    {
                        if (reminder.Reminding)
                        {
                            reminderStrings += reminder.Reminder + "\n";
                        }
                    }
                }

                turnMessages.Add(
                    new Message(title, round + counterStrings + effectStrings + reminderStrings)
                );
                return turnMessages;
            }

            return turnMessages;
        }
    }
}
