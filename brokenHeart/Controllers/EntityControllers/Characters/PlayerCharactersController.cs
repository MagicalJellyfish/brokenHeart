using Microsoft.AspNetCore.Mvc;
using brokenHeart.DB;
using brokenHeart.Entities.Characters;
using brokenHeart.Auxiliary;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Operations;
using brokenHeart.Entities.Stats;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Characters
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerCharactersController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public PlayerCharactersController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayerCharacters
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PlayerCharacter>>> GetPlayerCharacters()
        {
            if (_context.PlayerCharacters == null || _context.PlayerCharacters.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<PlayerCharacter> playerCharacters = _context.PlayerCharacters.Select(x => ApiAuxiliary.GetEntityPrepare(x) as PlayerCharacter).ToList();

            return Ok(playerCharacters);
        }

        // GET: api/PlayerCharacters/User/
        [HttpGet("User/{username}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PlayerCharacter>>> GetUsersPlayerCharacters(string username)
        {
            if (_context.PlayerCharacters == null || _context.PlayerCharacters.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<PlayerCharacter> playerCharacters = _context.PlayerCharacters.Where(x => x.Owner.Username == username).Select(x => ApiAuxiliary.GetEntityPrepare(x) as PlayerCharacter).ToList();

            return Ok(playerCharacters);
        }

        // GET: api/PlayerCharacters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerCharacter>> GetPlayerCharacter(int id)
        {
            if (_context.PlayerCharacters == null || _context.PlayerCharacters.Count() == 0)
            {
                return NotFound();
            }

            PlayerCharacter playerCharacter = ApiAuxiliary.GetEntityPrepare(await FullPlayerCharacters().FirstOrDefaultAsync(x => x.Id == id));

            if (playerCharacter == null)
            {
                return NotFound();
            }

            return playerCharacter;
        }

        // PATCH: api/PlayerCharacters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchPlayerCharacter(int id, JsonPatchDocument<PlayerCharacter> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            PlayerCharacter playerCharacter = FullPlayerCharacters().Single(x => x.Id == id);

            if(playerCharacter == null)
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
                ApiAuxiliary.PatchEntity(_context, typeof(PlayerCharacter), playerCharacter, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/PlayerCharacters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PlayerCharacter>> PostPlayerCharacter(PlayerCharacter playerCharacter)
        {
            foreach(Stat stat in _context.Stats)
            {
                playerCharacter.Stats.Add(new StatValue(stat, 0));
            }
            foreach (Bodypart bp in _context.Bodyparts)
            {
                playerCharacter.BodypartConditions.Add(new BodypartCondition(bp));
            }
            playerCharacter.Counters.Add(Constants.Dying.Instantiate());
            playerCharacter.Owner = _context.UserSimplified.Single(x => x.Username == User.Identity.Name);

            if (_context.PlayerCharacters == null)
            {
              return Problem("Entity set 'BrokenDbContext.PlayerCharacters'  is null.");
            }
            playerCharacter.Update();

            PlayerCharacter returnPlayerCharacter = ApiAuxiliary.PostEntity(_context, typeof(PlayerCharacter), playerCharacter);

            return CreatedAtAction("GetPlayerCharacter", new { id = returnPlayerCharacter.Id }, returnPlayerCharacter);
        }

        // DELETE: api/PlayerCharacters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerCharacter(int id)
        {
            if (_context.PlayerCharacters == null)
            {
                return NotFound();
            }
            var playerCharacter = await _context.PlayerCharacters.FindAsync(id);
            if (playerCharacter == null)
            {
                return NotFound();
            }

            _context.PlayerCharacters.Remove(playerCharacter);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<PlayerCharacter> FullPlayerCharacters()
        {
            return _context.PlayerCharacters
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
