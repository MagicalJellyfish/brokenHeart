using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities;
using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.Effects.Injuries;
using brokenHeart.Entities.Stats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Characters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public CharactersController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/Characters/Players
        [HttpGet("Players")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Character>>> GetPlayerCharacters()
        {
            if (_context.Characters == null || _context.Characters.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Character> characters = _context
                .Characters.Where(x => !x.IsNPC)
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as Character)
                .ToList();

            return Ok(characters);
        }

        // GET: api/Characters/NPC
        [HttpGet("NPC")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Character>>> GetNPCharacters()
        {
            if (_context.Characters == null || _context.Characters.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Character> characters = _context
                .Characters.Where(x => x.IsNPC)
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as Character)
                .ToList();

            return Ok(characters);
        }

        // GET: api/Characters/User/
        [HttpGet("User/{username}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Character>>> GetUsersCharacters(string username)
        {
            if (_context.Characters == null || _context.Characters.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Character> characters = _context
                .Characters.Where(x => x.Owner.Username == username)
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as Character)
                .ToList();

            return Ok(characters);
        }

        // GET: api/Characters/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Character>> GetCharacter(int id)
        {
            if (_context.Characters == null || _context.Characters.Count() == 0)
            {
                return NotFound();
            }

            Character character = ApiAuxiliary.GetEntityPrepare(
                await FullCharacters().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (character == null)
            {
                return NotFound();
            }

            return character;
        }

        // PATCH: api/Characters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchCharacters(
            int id,
            JsonPatchDocument<Character> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Character character = FullCharacters().Single(x => x.Id == id);

            if (character == null)
            {
                return NotFound($"No character with id {id} found!");
            }

            if (patchDocument.Operations.Any(x => x.path == "/defaultShortcut"))
            {
                if (
                    int.TryParse(
                        patchDocument
                            .Operations.First(x => x.path == "/defaultShortcut")
                            .value.ToString(),
                        out _
                    )
                )
                {
                    return BadRequest("Default Shortcut cannot be purely numbers.");
                }
            }

            List<Operation> operations = new List<Operation>();
            foreach (var operation in patchDocument.Operations)
            {
                operations.Add(operation);
            }

            try
            {
                ApiAuxiliary.PatchEntity(_context, typeof(Character), character, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Characters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Character>> PostCharacter(Character character)
        {
            if (int.TryParse(character.DefaultShortcut, out _))
            {
                return BadRequest("Default Shortcut cannot be purely numbers.");
            }

            foreach (Stat stat in _context.Stats)
            {
                character.Stats.Add(new StatValue(stat, 0));
            }
            foreach (Bodypart bp in _context.Bodyparts)
            {
                character.BodypartConditions.Add(new BodypartCondition(bp));
            }
            foreach (InjuryEffectTemplate injuryEffectTemplate in Constants.Bodyparts.InjuryEffects)
            {
                injuryEffectTemplate.Bodypart = _context.Bodyparts.Single(x =>
                    x.Id == injuryEffectTemplate.BodypartId
                );
                character.InjuryEffects.Add(injuryEffectTemplate.Instantiate());
            }

            character.Counters.Add(Constants.Dying.Instantiate());
            character.Owner = _context.UserSimplified.Single(x => x.Username == User.Identity.Name);

            if (_context.Characters == null)
            {
                return Problem("Entity set 'BrokenDbContext.Characters'  is null.");
            }

            Character returnCharacter = ApiAuxiliary.PostEntity(
                _context,
                typeof(Character),
                character
            );
            returnCharacter.Update();
            _context.SaveChanges();

            return CreatedAtAction(
                "GetCharacter",
                new { id = returnCharacter.Id },
                returnCharacter
            );
        }

        // DELETE: api/Characters/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            if (_context.Characters == null)
            {
                return NotFound();
            }
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            _context.Characters.Remove(character);
            _context.SaveChanges();

            return NoContent();
        }

        // PATCH: api/Characters/Bodyparts/0
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("Bodyparts/{id}")]
        [Authorize]
        public async Task<IActionResult> PatchBodyparts(
            int id,
            JsonPatchDocument<Character> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Character character = FullCharacters().Single(x => x.Id == id);

            if (character == null)
            {
                return BadRequest();
            }

            List<Effect> effects = character.Effects.ToList();
            effects.RemoveAll(x => character.InjuryEffects.Contains(x));
            character.Effects = effects;

            foreach (var operation in patchDocument.Operations)
            {
                BodypartCondition bpCon = character.BodypartConditions.SingleOrDefault(x =>
                    x.BodypartId == int.Parse(operation.path[1..])
                );
                InjuryLevel injury = (InjuryLevel)(long)operation.value;

                foreach (InjuryEffect injuryEffect in character.InjuryEffects)
                {
                    if (
                        injuryEffect.BodypartId == bpCon.BodypartId
                        && injuryEffect.InjuryLevel <= injury
                    )
                    {
                        character.Effects.Add(injuryEffect);
                    }
                }

                bpCon.InjuryLevel = injury;
            }

            _context.SaveChanges();

            return NoContent();
        }

        // csharpier-ignore
        private IQueryable<Character> FullCharacters()
        {
            return _context.Characters
                .Include(x => x.Abilities).ThenInclude(x => x.EffectTemplates)
                .Include(x => x.Abilities).ThenInclude(x => x.Rolls)

                .Include(x => x.Stats).ThenInclude(x => x.Stat)

                .Include(x => x.RoundReminders)

                .Include(x => x.Counters).ThenInclude(x => x.RoundReminder)

                .Include(x => x.BodypartConditions).ThenInclude(x => x.Bodypart)

                .Include(x => x.Effects).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Effects).ThenInclude(x => x.EffectCounter).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Effects).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Effects).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)

                .Include(x => x.InjuryEffects).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                .Include(x => x.InjuryEffects).ThenInclude(x => x.EffectCounter).ThenInclude(x => x.RoundReminder)
                .Include(x => x.InjuryEffects).ThenInclude(x => x.RoundReminder)
                .Include(x => x.InjuryEffects).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)

                .Include(x => x.Items).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Items).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Items).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)
                .Include(x => x.Items).ThenInclude(x => x.Abilities).ThenInclude(x => x.EffectTemplates)
                .Include(x => x.Items).ThenInclude(x => x.Abilities).ThenInclude(x => x.Rolls)

                .Include(x => x.Traits).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Traits).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Traits).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat);
        }
    }
}
