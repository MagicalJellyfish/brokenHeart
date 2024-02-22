using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities;
using brokenHeart.Entities.Combat;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.RoundReminders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombatController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public CombatController(BrokenDbContext brokenDbContext)
        {
            _context = brokenDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Combat>>> GetCombats()
        {
            if (_context.Combats == null || _context.Combats.Count() == 0)
            {
                return NotFound();
            }

            return FullCombats().ToList();
        }

        [HttpGet("active")]
        public async Task<ActionResult<Combat>> GetActiveCombat()
        {
            Combat activeCombat = ActiveCombat();

            if (activeCombat == null)
            {
                return NotFound();
            }

            return activeCombat;
        }

        [HttpPost]
        public async Task<ActionResult<int>> StartCombat()
        {
            Combat combat = new Combat();
            _context.Combats.Add(combat);

            foreach (Combat otherCombat in _context.Combats)
            {
                otherCombat.Active = false;
            }

            _context.SaveChanges();

            return combat.Id;
        }

        [HttpDelete]
        public async Task<IActionResult> EndCombat()
        {
            Combat? activeCombat = ActiveCombat();
            if (activeCombat == null)
            {
                return NotFound();
            }

            _context.Combats.Remove(activeCombat);
            return NoContent();
        }

        [HttpPatch("activate/{id}")]
        public async Task<IActionResult> ActivateCombat(int id)
        {
            foreach (Combat combat in _context.Combats)
            {
                if (combat.Id == id)
                {
                    combat.Active = true;
                }
                else
                {
                    combat.Active = false;
                }
            }

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("add-participant")]
        public async Task<ActionResult<object>> AddParticipant(int id, string? shortcut = null, int? initRoll = null)
        {
            Character? character = _context.Characters.Include(x => x.Stats).ThenInclude(x => x.Stat).SingleOrDefault(x => x.Id == id);

            if (character == null)
            {
                return NotFound();
            }

            if (shortcut == null)
            {
                if (character.DefaultShortcut != null)
                {
                    shortcut = character.DefaultShortcut;
                }
                else
                {
                    return BadRequest("Character has no default shortcut and no shortcut was given");
                }
            }

            Combat? activeCombat = ActiveCombat();
            if (activeCombat == null)
            {
                return BadRequest("No combat active");
            }

            if (activeCombat.Entries.Any(x => x.Shortcut == shortcut))
            {
                return BadRequest("Another participant already has this shortcut");
            }

            if (initRoll == null)
            {
                initRoll = RollAuxiliary.Roll(1, 20).Result + character.Stats.Single(x => x.Stat!.Id == Constants.Stats.Dex.Id).Value;
            }

            CombatEntry ce = new CombatEntry(character, (int)initRoll, shortcut);
            activeCombat.Entries.Add(ce);

            _context.SaveChanges();

            return new { character.Name, ce.Shortcut, ce.InitRoll };
        }

        [HttpPost("add-event")]
        public async Task<IActionResult> AddEvent(Event @event)
        {
            Combat? activeCombat = ActiveCombat();
            if (activeCombat == null)
            {
                return BadRequest("No combat active");
            }

            CombatEntry ce = new CombatEntry(@event, @event.Init, @event.Name);
            activeCombat.Entries.Add(ce);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("next-turn")]
        public async Task<ActionResult<List<Message>>> NextTurn()
        {
            List<Message> returnMessages = new List<Message>();
            Combat? combat = ActiveCombat();
            if (combat == null)
            {
                return NotFound();
            }

            combat.Entries = combat.Entries.OrderByDescending(x => x.InitRoll).ToList();

            if (combat.CurrentTurn == combat.Entries.Count - 1)
            {
                combat.CurrentTurn = -1;
                combat.Round += 1;

                returnMessages.Add(new Message("New Round!", "Current round is " + combat.Round + "!"));
            }
            else
            {
                returnMessages = BuildTurnMessage(combat);
            }

            _context.SaveChanges();

            return Ok(returnMessages);
        }

        private List<Message> BuildTurnMessage(Combat combat)
        {
            List<Message> turnMessages = new List<Message>();
            if (combat.CurrentTurn != combat.Entries.Count - 1)
            {
                combat.CurrentTurn += 1;
            }
            else
            {
                combat.CurrentTurn = -1;
                combat.Round += 1;

                turnMessages.Add(new Message("New Round!", "Current round is " + combat.Round + "!"));
                return turnMessages;
            }

            if (combat.Entries.ElementAt(combat.CurrentTurn).Event != null)
            {
                Event @event = combat.Entries.ElementAt(combat.CurrentTurn).Event;

                if (@event.Round == combat.Round)
                {
                    turnMessages.Add(new Message("Event " + @event.Name + " is happening!", "Round: " + combat.Round));
                }
                else
                {
                    if (!@event.Secret && @event.Round > combat.Round)
                    {
                        turnMessages.Add(new Message("Event " + @event.Name + " is advancing! It triggers in " + (@event.Round - combat.Round).ToString() + " rounds!", "Round: " + combat.Round));
                    }
                    turnMessages.AddRange(BuildTurnMessage(combat));
                }

                return turnMessages;
            }

            if (combat.Entries.ElementAt(combat.CurrentTurn).Character != null)
            {
                string round = "Round: " + combat.Round + "\n";
                string reminders = "Reminders: \n";

                Character character = _context.Characters
                    .Include(x => x.RoundReminders)
                    .Include(x => x.Counters).ThenInclude(x => x.RoundReminder)

                    .Include(x => x.Items).ThenInclude(x => x.RoundReminder)
                    .Include(x => x.Items).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)


                    .Include(x => x.Effects).ThenInclude(x => x.RoundReminder)
                    .Include(x => x.Effects).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                    .Include(x => x.Effects).ThenInclude(x => x.EffectCounter).ThenInclude(x => x.RoundReminder)

                    .Include(x => x.Traits).ThenInclude(x => x.RoundReminder)
                    .Include(x => x.Traits).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)

                    .SingleOrDefault(x => x.Id == combat.Entries.ElementAt(combat.CurrentTurn).Character.Id)!;

                string title = "It is " + character.Name + "'s turn! \n";

                string effects = "";
                foreach (Effect effect in character.Effects)
                {
                    if(!effect.Hp.IsNullOrEmpty())
                    {
                        //TODO: char-specific rolling
                        RollResult result = RollAuxiliary.RollString(effect.Hp);
                        effects += $"Your HP is changed by ({result.Detail}) {result.Result} from Effect \"{effect.Name}\"!";
                        character.Hp += result.Result;
                    }
                }

                List<RoundReminder?> reminderList = new List<RoundReminder?>();

                reminderList.AddRange(character.RoundReminders);
                reminderList.AddRange(character.Counters.Select(x => x.RoundReminder));

                reminderList.AddRange(character.Items.Select(x => x.RoundReminder));
                reminderList.AddRange(character.Items.SelectMany(x => x.Counters.Select(x => x.RoundReminder)));

                reminderList.AddRange(character.Effects.Select(x => x.RoundReminder));
                reminderList.AddRange(character.Effects.SelectMany(x => x.Counters.Select(x => x.RoundReminder)));
                reminderList.AddRange(character.Effects.Select(x => x.EffectCounter.RoundReminder));

                reminderList.AddRange(character.Traits.Select(x => x.RoundReminder));
                reminderList.AddRange(character.Traits.SelectMany(x => x.Counters.Select(x => x.RoundReminder)));

                if(reminderList.Count == 0 || reminderList.All(x => x == null))
                {
                    turnMessages.Add(new Message(title, round));
                    return turnMessages;
                }

                foreach(var reminder in reminderList)
                {
                    if(reminder != null)
                    {
                        if (reminder.Reminding)
                        {
                            reminders += reminder.Reminder + "\n";
                        }
                    }
                }

                turnMessages.Add(new Message(title, round + effects + reminders));
                return turnMessages;
            }

            return turnMessages;
        }

        [HttpDelete("remove-participant")]
        public async Task<IActionResult> RemoveParticipant(string shortcut)
        {
            Combat? activeCombat = ActiveCombat();
            if (activeCombat == null)
            {
                return BadRequest("No combat active");
            }

            CombatEntry? entry = activeCombat.Entries.SingleOrDefault(x => x.Shortcut == shortcut);
            if(entry == null)
            {
                return NotFound();
            }

            if (activeCombat.Entries.ToList().FindIndex(x => x.Shortcut == shortcut) <= activeCombat.CurrentTurn)
            {
                activeCombat.CurrentTurn -= 1;
            }

            activeCombat.Entries.Remove(entry);

            _context.SaveChanges();

            return NoContent();
        }

        private Combat? ActiveCombat()
        {
            if (_context.Counters == null || _context.Counters.Count() == 0)
            {
                return null;
            }

            Combat? combat = FullCombats().SingleOrDefault(x => x.Active == true);

            return combat;
        }

        private IQueryable<Combat> FullCombats()
        {
            return _context.Combats
                .Include(x => x.Entries)
                    .ThenInclude(x => x.Character)
                .Include(x => x.Entries)
                    .ThenInclude(x => x.Event);
        }
    }
}
