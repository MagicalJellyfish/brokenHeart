using brokenHeart.Auxiliary;
using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.TraitTemplates
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraitTemplatesController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public TraitTemplatesController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/TraitTemplates
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TraitTemplate>>> GetTraitTemplates()
        {
            if (_context.TraitTemplates == null || _context.TraitTemplates.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<TraitTemplate> traitTemplates = FullTraitTemplates()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as TraitTemplate)
                .ToList();

            return Ok(traitTemplates);
        }

        // GET: api/TraitTemplates/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TraitTemplate>> GetTraitTemplate(int id)
        {
            if (_context.TraitTemplates == null || _context.TraitTemplates.Count() == 0)
            {
                return NotFound();
            }

            TraitTemplate traitTemplate = ApiAuxiliary.GetEntityPrepare(
                await FullTraitTemplates().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (traitTemplate == null)
            {
                return NotFound();
            }

            return traitTemplate;
        }

        // PATCH: api/TraitTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchTraitTemplate(
            int id,
            JsonPatchDocument<TraitTemplate> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            TraitTemplate traitTemplate = FullTraitTemplates().Single(x => x.Id == id);

            if (traitTemplate == null)
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
                    typeof(TraitTemplate),
                    traitTemplate,
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

        // POST: api/TraitTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TraitTemplate>> PostTraitTemplate(
            TraitTemplate traitTemplate
        )
        {
            if (_context.TraitTemplates == null)
            {
                return Problem("Entity set 'BrokenDbContext.TraitTemplates'  is null.");
            }

            TraitTemplate returnTraitTemplate = ApiAuxiliary.PostEntity(
                _context,
                typeof(TraitTemplate),
                traitTemplate
            );

            return CreatedAtAction(
                "GetTraitTemplate",
                new { id = returnTraitTemplate.Id },
                returnTraitTemplate
            );
        }

        // DELETE: api/TraitTemplates/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTraitTemplate(int id)
        {
            if (_context.TraitTemplates == null)
            {
                return NotFound();
            }
            var traitTemplate = await _context.TraitTemplates.FindAsync(id);
            if (traitTemplate == null)
            {
                return NotFound();
            }

            _context.TraitTemplates.Remove(traitTemplate);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/TraitTemplates/Instantiate/5
        [HttpGet("Instantiate/{id}")]
        [Authorize]
        public async Task<ActionResult<Trait>> InstantiateTraitTemplate(int id)
        {
            if (_context.TraitTemplates == null || _context.TraitTemplates.Count() == 0)
            {
                return NotFound();
            }

            TraitTemplate traitTemplate = FullTraitTemplates().Single(x => x.Id == id);

            if (traitTemplate == null)
            {
                return NotFound();
            }

            Trait trait = traitTemplate.Instantiate();

            return trait;
        }

        private IQueryable<TraitTemplate> FullTraitTemplates()
        {
            return _context
                .TraitTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.CounterTemplates)
                .Include(x => x.RoundReminderTemplate)
                .Include(x => x.StatIncreases)
                .ThenInclude(x => x.Stat);
        }
    }
}
