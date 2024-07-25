using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.RoundReminders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.EffectCounterTemplates
{
    [Route("api/[controller]")]
    [ApiController]
    public class EffectCounterTemplatesController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public EffectCounterTemplatesController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/EffectCounterTemplates
        [HttpGet]
        [Authorize]
        public async Task<
            ActionResult<IEnumerable<EffectCounterTemplate>>
        > GetEffectCounterTemplates()
        {
            if (
                _context.EffectCounterTemplates == null
                || _context.EffectCounterTemplates.Count() == 0
            )
            {
                return NotFound();
            }

            IEnumerable<EffectCounterTemplate> effectCounterTemplates = FullEffectCounterTemplates()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as EffectCounterTemplate)
                .ToList();

            return Ok(effectCounterTemplates);
        }

        // GET: api/EffectCounterTemplates/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<EffectCounterTemplate>> GetEffectCounterTemplate(int id)
        {
            if (
                _context.EffectCounterTemplates == null
                || _context.EffectCounterTemplates.Count() == 0
            )
            {
                return NotFound();
            }

            EffectCounterTemplate effectCounterTemplate = ApiAuxiliary.GetEntityPrepare(
                await FullEffectCounterTemplates().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (effectCounterTemplate == null)
            {
                return NotFound();
            }

            return effectCounterTemplate;
        }

        // PATCH: api/EffectCounterTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchEffectCounterTemplate(
            int id,
            JsonPatchDocument<EffectCounterTemplate> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            EffectCounterTemplate effectCounterTemplate = FullEffectCounterTemplates()
                .Single(x => x.Id == id);

            if (effectCounterTemplate == null)
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
                    typeof(EffectCounterTemplate),
                    effectCounterTemplate,
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

        // POST: api/EffectCounterTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EffectCounterTemplate>> PostEffectCounterTemplate(
            EffectCounterTemplate effectCounterTemplate
        )
        {
            if (_context.EffectCounterTemplates == null)
            {
                return Problem("Entity set 'BrokenDbContext.EffectCounterTemplates'  is null.");
            }

            EffectCounterTemplate returnEffectCounterTemplate = ApiAuxiliary.PostEntity(
                _context,
                typeof(EffectCounterTemplate),
                effectCounterTemplate
            );

            return CreatedAtAction(
                "GetEffectCounterTemplate",
                new { id = returnEffectCounterTemplate.Id },
                returnEffectCounterTemplate
            );
        }

        // DELETE: api/EffectCounterTemplates/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEffectCounterTemplate(int id)
        {
            if (_context.EffectCounterTemplates == null)
            {
                return NotFound();
            }
            var effectCounterTemplate = await _context.EffectCounterTemplates.FindAsync(id);
            if (effectCounterTemplate == null)
            {
                return NotFound();
            }

            _context.EffectCounterTemplates.Remove(effectCounterTemplate);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/EffectCounterTemplates/Instantiate/5
        [HttpGet("Instantiate/{id}")]
        [Authorize]
        public async Task<ActionResult<EffectCounter>> InstantiateEffectCounterTemplate(int id)
        {
            if (
                _context.EffectCounterTemplates == null
                || _context.EffectCounterTemplates.Count() == 0
            )
            {
                return NotFound();
            }

            EffectCounterTemplate effectCounterTemplate = FullEffectCounterTemplates()
                .Single(x => x.Id == id);

            if (effectCounterTemplate == null)
            {
                return NotFound();
            }

            EffectCounter effectCounter = effectCounterTemplate.Instantiate();

            return effectCounter;
        }

        private IQueryable<EffectCounterTemplate> FullEffectCounterTemplates()
        {
            return _context
                .EffectCounterTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.RoundReminderTemplate);
        }
    }
}
