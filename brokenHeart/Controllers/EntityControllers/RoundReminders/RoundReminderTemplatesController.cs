using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Traits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.RoundReminderTemplates
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundReminderTemplatesController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public RoundReminderTemplatesController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/RoundReminderTemplates
        [HttpGet]
        [Authorize]
        public async Task<
            ActionResult<IEnumerable<RoundReminderTemplate>>
        > GetRoundReminderTemplates()
        {
            if (
                _context.RoundReminderTemplates == null
                || _context.RoundReminderTemplates.Count() == 0
            )
            {
                return NotFound();
            }

            IEnumerable<RoundReminderTemplate> roundReminderTemplates = FullRoundReminderTemplates()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as RoundReminderTemplate)
                .ToList();

            return Ok(roundReminderTemplates);
        }

        // GET: api/RoundReminderTemplates/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RoundReminderTemplate>> GetRoundReminderTemplate(int id)
        {
            if (
                _context.RoundReminderTemplates == null
                || _context.RoundReminderTemplates.Count() == 0
            )
            {
                return NotFound();
            }

            RoundReminderTemplate roundReminderTemplate = ApiAuxiliary.GetEntityPrepare(
                await FullRoundReminderTemplates().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (roundReminderTemplate == null)
            {
                return NotFound();
            }

            return roundReminderTemplate;
        }

        // PATCH: api/RoundReminderTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchRoundReminderTemplate(
            int id,
            JsonPatchDocument<RoundReminderTemplate> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            RoundReminderTemplate roundReminderTemplate = FullRoundReminderTemplates()
                .Single(x => x.Id == id);

            if (roundReminderTemplate == null)
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
                    typeof(RoundReminderTemplate),
                    roundReminderTemplate,
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

        // POST: api/RoundReminderTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RoundReminderTemplate>> PostRoundReminderTemplate(
            RoundReminderTemplate roundReminderTemplate
        )
        {
            if (_context.RoundReminderTemplates == null)
            {
                return Problem("Entity set 'BrokenDbContext.RoundReminderTemplates'  is null.");
            }

            RoundReminderTemplate returnRoundReminderTemplate = ApiAuxiliary.PostEntity(
                _context,
                typeof(RoundReminderTemplate),
                roundReminderTemplate
            );

            return CreatedAtAction(
                "GetRoundReminderTemplate",
                new { id = returnRoundReminderTemplate.Id },
                returnRoundReminderTemplate
            );
        }

        // DELETE: api/RoundReminderTemplates/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRoundReminderTemplate(int id)
        {
            if (_context.RoundReminderTemplates == null)
            {
                return NotFound();
            }
            var roundReminderTemplate = await _context.RoundReminderTemplates.FindAsync(id);
            if (roundReminderTemplate == null)
            {
                return NotFound();
            }

            if (roundReminderTemplate.Id <= 15)
            {
                return BadRequest("Integral Reminder-Templates can't be deleted");
            }

            _context.RoundReminderTemplates.Remove(roundReminderTemplate);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/RoundReminderTemplates/Instantiate/5
        [HttpGet("Instantiate/{id}")]
        [Authorize]
        public async Task<ActionResult<RoundReminder>> InstantiateRoundReminderTemplate(int id)
        {
            if (
                _context.RoundReminderTemplates == null
                || _context.RoundReminderTemplates.Count() == 0
            )
            {
                return NotFound();
            }

            RoundReminderTemplate roundReminderTemplate =
                await _context.RoundReminderTemplates.FindAsync(id);

            if (roundReminderTemplate == null)
            {
                return NotFound();
            }

            RoundReminder roundReminder = roundReminderTemplate.Instantiate();

            return roundReminder;
        }

        private IQueryable<RoundReminderTemplate> FullRoundReminderTemplates()
        {
            return _context
                .RoundReminderTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.ModifierTemplates);
        }
    }
}
