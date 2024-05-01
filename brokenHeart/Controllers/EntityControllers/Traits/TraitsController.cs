using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.Traits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Traits
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraitsController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public TraitsController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/Traits
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Trait>>> GetTraits()
        {
            if (_context.Traits == null || _context.Traits.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Trait> traits = FullTraits()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as Trait)
                .ToList();

            return Ok(traits);
        }

        // GET: api/Traits/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Trait>> GetTrait(int id)
        {
            if (_context.Traits == null || _context.Traits.Count() == 0)
            {
                return NotFound();
            }

            Trait trait = ApiAuxiliary.GetEntityPrepare(
                await FullTraits().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (trait == null)
            {
                return NotFound();
            }

            return trait;
        }

        // PATCH: api/Traits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchTrait(int id, JsonPatchDocument<Trait> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Trait trait = FullTraits().Single(x => x.Id == id);

            if (trait == null)
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
                ApiAuxiliary.PatchEntity(_context, typeof(Trait), trait, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Traits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Trait>> PostTrait(Trait trait)
        {
            if (_context.Traits == null)
            {
                return Problem("Entity set 'BrokenDbContext.Traits'  is null.");
            }

            Trait returnTrait = ApiAuxiliary.PostEntity(_context, typeof(Trait), trait);

            return CreatedAtAction("GetTrait", new { id = returnTrait.Id }, returnTrait);
        }

        // DELETE: api/Traits/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTrait(int id)
        {
            if (_context.Traits == null)
            {
                return NotFound();
            }
            var trait = await _context.Traits.FindAsync(id);
            if (trait == null)
            {
                return NotFound();
            }

            _context.Traits.Remove(trait);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<Trait> FullTraits()
        {
            return _context
                .Traits.Include(x => x.Counters)
                .Include(x => x.RoundReminder)
                .Include(x => x.StatIncreases)
                .ThenInclude(x => x.Stat);
        }
    }
}
