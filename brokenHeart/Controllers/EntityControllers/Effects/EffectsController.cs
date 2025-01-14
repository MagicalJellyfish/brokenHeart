using brokenHeart.Auxiliary;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Effects
{
    [Route("api/[controller]")]
    [ApiController]
    public class EffectsController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public EffectsController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/Effects
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Effect>>> GetEffects()
        {
            if (_context.Effects == null || _context.Effects.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Effect> effects = FullEffects()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as Effect)
                .ToList();

            return Ok(effects);
        }

        // GET: api/Effects/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Effect>> GetEffect(int id)
        {
            if (_context.Effects == null || _context.Effects.Count() == 0)
            {
                return NotFound();
            }

            Effect effect = ApiAuxiliary.GetEntityPrepare(
                await FullEffects().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (effect == null)
            {
                return NotFound();
            }

            return effect;
        }

        // PATCH: api/Effects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchEffect(
            int id,
            JsonPatchDocument<Effect> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Effect effect = FullEffects().Single(x => x.Id == id);

            if (effect == null)
            {
                return BadRequest();
            }

            List<Operation> operations = new List<Operation>();
            foreach (var operation in patchDocument.Operations)
            {
                operations.Add(operation);
            }

            try
            {
                ApiAuxiliary.PatchEntity(_context, typeof(Effect), effect, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Effects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Effect>> PostEffect(Effect effect)
        {
            if (_context.Effects == null)
            {
                return Problem("Entity set 'BrokenDbContext.Effects'  is null.");
            }

            Effect returnEffect = ApiAuxiliary.PostEntity(_context, typeof(Effect), effect);

            return CreatedAtAction("GetEffect", new { id = returnEffect.Id }, returnEffect);
        }

        // DELETE: api/Effects/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEffect(int id)
        {
            if (_context.Effects == null)
            {
                return NotFound();
            }
            var effect = await _context.Effects.FindAsync(id);
            if (effect == null)
            {
                return NotFound();
            }

            if (_context.InjuryEffects.Any(x => x.Id == effect.Id))
            {
                return BadRequest("Cannot delete InjuryEffect");
            }

            _context.Effects.Remove(effect);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<Effect> FullEffects()
        {
            return _context
                .Effects.Include(x => x.Counters)
                .Include(x => x.EffectCounter)
                .Include(x => x.RoundReminder)
                .Include(x => x.StatIncreases)
                .ThenInclude(x => x.Stat)
                .Include(x => x.Abilities);
        }
    }
}
