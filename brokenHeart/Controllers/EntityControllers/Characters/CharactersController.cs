using Microsoft.AspNetCore.Mvc;
using brokenHeart.DB;
using brokenHeart.Entities.Characters;
using brokenHeart.Auxiliary;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Operations;
using brokenHeart.Entities.Stats;
using Microsoft.EntityFrameworkCore;
using brokenHeart.Entities;

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

        // GET: api/Characters
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharacters()
        {
            if (_context.Characters == null || _context.Characters.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Character> characters = _context.Characters.Select(x => ApiAuxiliary.GetEntityPrepare(x) as Character).ToList();

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

            IEnumerable<Character> characters = _context.Characters.Where(x => x.Owner.Username == username).Select(x => ApiAuxiliary.GetEntityPrepare(x) as Character).ToList();

            return Ok(characters);
        }

        // GET: api/Characters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Character>> GetCharacters(int id)
        {
            if (_context.Characters == null || _context.Characters.Count() == 0)
            {
                return NotFound();
            }

            Character character = ApiAuxiliary.GetEntityPrepare(await FullCharacters().FirstOrDefaultAsync(x => x.Id == id));

            if (character == null)
            {
                return NotFound();
            }

            return character;
        }

        // PATCH: api/Characters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCharacters(int id, JsonPatchDocument<Character> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Character character = FullCharacters().Single(x => x.Id == id);

            if(character == null)
            {
                return BadRequest();
            }

            List<Operation> operations = new List<Operation>();
            foreach(var operation in patchDocument.Operations)
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
            foreach(Stat stat in _context.Stats)
            {
                character.Stats.Add(new StatValue(stat, 0));
            }
            foreach (Bodypart bp in _context.Bodyparts)
            {
                character.BodypartConditions.Add(new BodypartCondition(bp));
            }
            character.Counters.Add(Constants.Dying.Instantiate());
            character.Owner = _context.UserSimplified.Single(x => x.Username == User.Identity.Name);

            if (_context.Characters == null)
            {
              return Problem("Entity set 'BrokenDbContext.Characters'  is null.");
            }
            character.Update();

            Character returnCharacter = ApiAuxiliary.PostEntity(_context, typeof(Character), character);

            return CreatedAtAction("GetCharacters", new { id = returnCharacter.Id }, returnCharacter);
        }

        // DELETE: api/Characters/5
        [HttpDelete("{id}")]
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

        [HttpPatch("activate")]
        public async Task<ActionResult<string>> ActivateCharacter(ulong discordId, int charId)
        {
            UserSimplified user = _context.UserSimplified.Include(x => x.Characters).Include(x => x.ActiveCharacter).SingleOrDefault(x => x.DiscordId == discordId);

            if(user == null)
            {
                return NotFound();
            }

            Character character = user.Characters.SingleOrDefault(x => x.Id == charId);

            if(character == null)
            {
                return NotFound();
            }

            user.ActiveCharacter = character;
            _context.SaveChanges();
            return character.Name;
        }

        private IQueryable<Character> FullCharacters()
        {
            return _context.Characters
                .Include(x => x.Stats).ThenInclude(x => x.Stat)
                .Include(x => x.RoundReminders)
                .Include(x => x.Counters).ThenInclude(x => x.RoundReminder)
                .Include(x => x.BodypartConditions).ThenInclude(x => x.Bodypart)

                .Include(x => x.Effects).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Effects).ThenInclude(x => x.EffectCounter).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Effects).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Effects).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)

                .Include(x => x.Items).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Items).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Items).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)

                .Include(x => x.Traits).ThenInclude(x => x.Counters).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Traits).ThenInclude(x => x.RoundReminder)
                .Include(x => x.Traits).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)
                ;
        }
    }
}
