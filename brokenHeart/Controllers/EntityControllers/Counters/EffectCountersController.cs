using System.Diagnostics.Metrics;
using brokenHeart.Auxiliary;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.EffectCounters
{
    [Route("api/[controller]")]
    [ApiController]
    public class EffectCountersController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public EffectCountersController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/EffectCounters
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EffectCounter>>> GetEffectCounters()
        {
            if (_context.EffectCounters == null || _context.EffectCounters.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<EffectCounter> effectCounters = FullEffectCounters()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as EffectCounter)
                .ToList();

            return Ok(effectCounters);
        }

        // GET: api/EffectCounters/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<EffectCounter>> GetEffectCounter(int id)
        {
            if (_context.EffectCounters == null || _context.EffectCounters.Count() == 0)
            {
                return NotFound();
            }

            EffectCounter effectCounter = ApiAuxiliary.GetEntityPrepare(
                await FullEffectCounters().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (effectCounter == null)
            {
                return NotFound();
            }

            return effectCounter;
        }

        // PATCH: api/EffectCounters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchEffectCounter(
            int id,
            JsonPatchDocument<EffectCounter> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            EffectCounter effectCounter = FullEffectCounters().Single(x => x.Id == id);

            if (effectCounter == null)
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
                ApiAuxiliary.PatchEntity(
                    _context,
                    typeof(EffectCounter),
                    effectCounter,
                    operations
                );
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/EffectCounters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EffectCounter>> PostEffectCounter(
            EffectCounter effectCounter
        )
        {
            if (_context.EffectCounters == null)
            {
                return Problem("Entity set 'BrokenDbContext.EffectCounters'  is null.");
            }

            EffectCounter returnEffectCounter = ApiAuxiliary.PostEntity(
                _context,
                typeof(EffectCounter),
                effectCounter
            );

            return CreatedAtAction(
                "GetEffectCounter",
                new { id = returnEffectCounter.Id },
                returnEffectCounter
            );
        }

        // DELETE: api/EffectCounters/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEffectCounter(int id)
        {
            if (_context.EffectCounters == null)
            {
                return NotFound();
            }
            var effectCounter = await _context.EffectCounters.FindAsync(id);
            if (effectCounter == null)
            {
                return NotFound();
            }

            _context.EffectCounters.Remove(effectCounter);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<EffectCounter> FullEffectCounters()
        {
            return _context.EffectCounters.Include(x => x.RoundReminder);
        }
    }
}
