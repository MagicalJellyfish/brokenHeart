using brokenHeart.Database.DAO.Combat;
using brokenHeart.DB;
using brokenHeart.Models.brokenHand;
using brokenHeart.Services;
using brokenHeart.Services.Logic.CombatTracking;
using brokenHeart.Services.Rolling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Localhost]
    public class CombatController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly ICharacterRollService _charRollService;
        private readonly ITurnAdvancementService _turnAdvancementService;

        public CombatController(
            BrokenDbContext brokenDbContext,
            ICharacterRollService charRollService,
            ITurnAdvancementService turnAdvancementService
        )
        {
            _context = brokenDbContext;
            _charRollService = charRollService;
            _turnAdvancementService = turnAdvancementService;
        }

        [HttpPost]
        public async Task<ActionResult<int>> StartCombat()
        {
            foreach (Combat otherCombat in _context.Combats)
            {
                otherCombat.Active = false;
            }

            Combat combat = new Combat();
            _context.Combats.Add(combat);

            _context.SaveChanges();

            return combat.Id;
        }

        [HttpDelete]
        public async Task<IActionResult> EndCombat()
        {
            Combat? activeCombat = _context.Combats.SingleOrDefault(x => x.Active);
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
        public async Task<ActionResult<object>> AddParticipant(
            int id,
            string? shortcut = null,
            int? initRoll = null
        )
        {
            var character = _context
                .Characters.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.DefaultShortcut,
                })
                .SingleOrDefault(x => x.Id == id);

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
                    return BadRequest(
                        "Character has no default shortcut and no shortcut was given"
                    );
                }
            }

            Combat? activeCombat = _context.Combats.SingleOrDefault(x => x.Active);
            if (activeCombat == null)
            {
                return BadRequest("No combat active");
            }

            if (
                _context
                    .Combats.Where(x => x.Id == activeCombat.Id)
                    .SelectMany(x => x.Entries)
                    .Any(x => x.Shortcut == shortcut)
            )
            {
                return BadRequest("Another participant already has this shortcut");
            }

            if (initRoll == null)
            {
                initRoll = _charRollService.CharRollString("1d20+INS", character.Id).Result;
            }

            CombatEntry ce = new CombatEntry(
                activeCombat.Id,
                character.Id,
                (int)initRoll,
                _context
                    .Characters.Where(x => x.Id == character.Id)
                    .Select(x => x.Stats.Single(x => x.Stat.Name.ToLower().StartsWith("ins")).Value)
                    .Single(),
                shortcut
            );
            _context.CombatEntries.Add(ce);

            _context.SaveChanges();

            return new AddParticipantMessage()
            {
                Name = character.Name,
                Shortcut = ce.Shortcut,
                InitRoll = ce.InitRoll,
            };
        }

        [HttpPost("add-event")]
        public async Task<IActionResult> AddEvent(Event @event)
        {
            Combat? activeCombat = _context.Combats.SingleOrDefault(x => x.Active);
            if (activeCombat == null)
            {
                return BadRequest("No combat active");
            }

            CombatEntry ce = new CombatEntry(activeCombat.Id, @event, @event.Init, @event.Name);
            _context.CombatEntries.Add(ce);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("next-turn")]
        public async Task<ActionResult<List<Message>>> NextTurn()
        {
            List<Message> returnMessages = new List<Message>();
            Combat? combat = _context.Combats.SingleOrDefault(x => x.Active);
            if (combat == null)
            {
                return NotFound();
            }

            List<CombatEntry> orderedEntries = SortEntries(
                _context.CombatEntries.Where(x => x.CombatId == combat.Id).ToList()
            );

            returnMessages = _turnAdvancementService.AdvanceTurn(combat, orderedEntries);

            _context.SaveChanges();

            return Ok(returnMessages);
        }

        [HttpDelete("remove-participant")]
        public async Task<IActionResult> RemoveParticipant(string shortcut)
        {
            Combat? activeCombat = _context
                .Combats.Include(x => x.Entries)
                .SingleOrDefault(x => x.Active);
            if (activeCombat == null)
            {
                return BadRequest("No combat active");
            }

            CombatEntry? entry = activeCombat.Entries.SingleOrDefault(x => x.Shortcut == shortcut);
            if (entry == null)
            {
                return NotFound();
            }

            if (
                SortEntries(activeCombat.Entries).FindIndex(x => x.Shortcut == shortcut)
                <= activeCombat.CurrentTurn
            )
            {
                activeCombat.CurrentTurn -= 1;
            }

            activeCombat.Entries.Remove(entry);

            _context.SaveChanges();

            return NoContent();
        }

        private List<CombatEntry> SortEntries(ICollection<CombatEntry> entries)
        {
            return entries
                .OrderByDescending(x => x.InitRoll)
                .ThenByDescending(x => x.InitStat)
                .ThenByDescending(x => x.CharacterId)
                .ThenByDescending(x => x.EventId)
                .ToList();
        }
    }
}
