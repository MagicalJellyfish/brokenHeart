using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities;
using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Combat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

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

        [HttpGet]
        [Route("active")]
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

        [HttpPatch]
        [Route("activate/{id}")]
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

        [HttpPost]
        [Route("add-participant")]
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
            activeCombat.Entries = activeCombat.Entries.OrderByDescending(x => x.InitRoll).ToList();

            _context.SaveChanges();

            return new { character.Name, ce.Shortcut, ce.InitRoll };
        }

        [HttpPost]
        [Route("add-event")]
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

        [HttpPatch]
        [Route("nextTurn")]
        public async Task<ActionResult<string>> NextTurn()
        {
            string returnMessage = "";
            Combat? combat = ActiveCombat();
            if (combat == null)
            {
                return NotFound();
            }

            if(combat.CurrentTurn == combat.Entries.Count - 1)
            {
                returnMessage += "Next Round!\n";
                combat.CurrentTurn = -1;
                combat.Round += 1;

                returnMessage += "Current round is " + combat.Round + "!";
            }
            else
            {
                combat.CurrentTurn += 1;

                returnMessage += "Round: " + combat.Round + "\n";

                if (combat.Entries.ElementAt(combat.CurrentTurn).Event != null)
                {
                    Event @event = combat.Entries.ElementAt(combat.CurrentTurn).Event;

                    if (@event.Round == combat.Round)
                    {
                        returnMessage += "Event " + @event.Name + " is happening! \n";
                    }
                    else
                    {
                        if (!@event.Secret && @event.Round > combat.Round)
                        {
                            returnMessage += "Event " + @event.Name + " is advancing! It triggers in " + (@event.Round - combat.Round).ToString() + " rounds! \n";
                        }
                        //TODO: extract everything after advanding turn to separate function, call it again here since an event, if it doesn't trigger, should just add "triggering in x rounds" to beginning of next turn
                    }
                }

                if (combat.Entries.ElementAt(combat.CurrentTurn).Character != null)
                {
                    Character character = combat.Entries.ElementAt(combat.CurrentTurn).Character;
                    returnMessage += "It is " + character.Name + "'s turn! \n";


                    //TODO: Get all reminders
                }
            }

            _context.SaveChanges();

            return Ok(returnMessage);
        }

        [HttpDelete]
        [Route("remove-participant")]
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
