using System.Diagnostics.Metrics;
using brokenHeart.Auxiliary;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.CounterTemplates
{
    [Route("api/[controller]")]
    [ApiController]
    public class CounterTemplatesController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public CounterTemplatesController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/CounterTemplates
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CounterTemplate>>> GetCounterTemplates()
        {
            if (_context.CounterTemplates == null || _context.CounterTemplates.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<CounterTemplate> counterTemplates = FullCounterTemplates()
                .Where(x => EF.Property<string>(x, "Discriminator") == "CounterTemplate")
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as CounterTemplate)
                .ToList();

            return Ok(counterTemplates);
        }

        // GET: api/CounterTemplates/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CounterTemplate>> GetCounterTemplate(int id)
        {
            if (_context.CounterTemplates == null || _context.CounterTemplates.Count() == 0)
            {
                return NotFound();
            }

            CounterTemplate counterTemplate = ApiAuxiliary.GetEntityPrepare(
                await FullCounterTemplates().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (counterTemplate == null)
            {
                return NotFound();
            }

            return counterTemplate;
        }

        // PATCH: api/CounterTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchCounterTemplate(
            int id,
            JsonPatchDocument<CounterTemplate> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            CounterTemplate counterTemplate = FullCounterTemplates().Single(x => x.Id == id);

            if (counterTemplate == null)
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
                    typeof(CounterTemplate),
                    counterTemplate,
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

        // POST: api/CounterTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CounterTemplate>> PostCounterTemplate(
            CounterTemplate counterTemplate
        )
        {
            if (_context.CounterTemplates == null)
            {
                return Problem("Entity set 'BrokenDbContext.CounterTemplates'  is null.");
            }

            CounterTemplate returnCounterTemplate = ApiAuxiliary.PostEntity(
                _context,
                typeof(CounterTemplate),
                counterTemplate
            );

            return CreatedAtAction(
                "GetCounterTemplate",
                new { id = returnCounterTemplate.Id },
                returnCounterTemplate
            );
        }

        // DELETE: api/CounterTemplates/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCounterTemplate(int id)
        {
            if (_context.CounterTemplates == null)
            {
                return NotFound();
            }
            var counterTemplate = await _context.CounterTemplates.FindAsync(id);
            if (counterTemplate == null)
            {
                return NotFound();
            }

            if (counterTemplate.Id <= 1)
            {
                return BadRequest("Integral Counter-Templates can't be deleted");
            }

            _context.CounterTemplates.Remove(counterTemplate);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/CounterTemplates/Instantiate/5
        [HttpGet("Instantiate/{id}")]
        [Authorize]
        public async Task<ActionResult<Counter>> InstantiateCounterTemplate(int id)
        {
            if (_context.CounterTemplates == null || _context.CounterTemplates.Count() == 0)
            {
                return NotFound();
            }

            CounterTemplate counterTemplate = FullCounterTemplates().Single(x => x.Id == id);

            if (counterTemplate == null)
            {
                return NotFound();
            }

            Counter counter = counterTemplate.Instantiate();

            return counter;
        }

        private IQueryable<CounterTemplate> FullCounterTemplates()
        {
            return _context
                .CounterTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.RoundReminderTemplate);
        }
    }
}
