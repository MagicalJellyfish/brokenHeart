using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Services.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.EffectTemplates
{
    [Route("api/[controller]")]
    [ApiController]
    public class EffectTemplatesController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly IEndpointEntityService _endpointEntityService;

        public EffectTemplatesController(
            BrokenDbContext context,
            IEndpointEntityService endpointEntityService
        )
        {
            _context = context;
            _endpointEntityService = endpointEntityService;
        }

        // GET: api/EffectTemplates
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EffectTemplate>>> GetEffectTemplates()
        {
            if (_context.EffectTemplates == null || _context.EffectTemplates.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<EffectTemplate> effectTemplates = FullEffectTemplates()
                .Select(x => _endpointEntityService.GetEntityPrepare(x) as EffectTemplate)
                .ToList();

            return Ok(effectTemplates);
        }

        // GET: api/EffectTemplates/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<EffectTemplate>> GetEffectTemplate(int id)
        {
            if (_context.EffectTemplates == null || _context.EffectTemplates.Count() == 0)
            {
                return NotFound();
            }

            EffectTemplate effectTemplate = _endpointEntityService.GetEntityPrepare(
                await FullEffectTemplates().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (effectTemplate == null)
            {
                return NotFound();
            }

            return effectTemplate;
        }

        // PATCH: api/EffectTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchEffectTemplate(
            int id,
            JsonPatchDocument<EffectTemplate> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            EffectTemplate effectTemplate = FullEffectTemplates().Single(x => x.Id == id);

            if (effectTemplate == null)
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
                _endpointEntityService.PatchEntity(
                    _context,
                    typeof(EffectTemplate),
                    effectTemplate,
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

        // POST: api/EffectTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EffectTemplate>> PostEffectTemplate(
            EffectTemplate effectTemplate
        )
        {
            if (_context.EffectTemplates == null)
            {
                return Problem("Entity set 'BrokenDbContext.EffectTemplates'  is null.");
            }

            EffectTemplate returnEffectTemplate = _endpointEntityService.PostEntity(
                _context,
                typeof(EffectTemplate),
                effectTemplate
            );

            return CreatedAtAction(
                "GetEffectTemplate",
                new { id = returnEffectTemplate.Id },
                returnEffectTemplate
            );
        }

        // DELETE: api/EffectTemplates/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEffectTemplate(int id)
        {
            if (_context.EffectTemplates == null)
            {
                return NotFound();
            }
            var effectTemplate = await _context.EffectTemplates.FindAsync(id);
            if (effectTemplate == null)
            {
                return NotFound();
            }

            if (_context.InjuryEffectTemplates.Any(x => x.Id == effectTemplate.Id))
            {
                return BadRequest("Cannot delete InjuryEffectTemplates");
            }

            _context.EffectTemplates.Remove(effectTemplate);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/EffectTemplates/Instantiate/5
        [HttpGet("Instantiate/{id}")]
        [Authorize]
        public async Task<ActionResult<Effect>> InstantiateEffectTemplate(int id)
        {
            if (_context.EffectTemplates == null || _context.EffectTemplates.Count() == 0)
            {
                return NotFound();
            }

            EffectTemplate effectTemplate = FullEffectTemplates().Single(x => x.Id == id);

            if (effectTemplate == null)
            {
                return NotFound();
            }

            Effect effect = effectTemplate.Instantiate();

            return effect;
        }

        private IQueryable<EffectTemplate> FullEffectTemplates()
        {
            return _context
                .EffectTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.CounterTemplates)
                .Include(x => x.EffectCounterTemplate)
                .Include(x => x.RoundReminderTemplate)
                .Include(x => x.StatIncreases)
                .ThenInclude(x => x.Stat);
        }
    }
}
