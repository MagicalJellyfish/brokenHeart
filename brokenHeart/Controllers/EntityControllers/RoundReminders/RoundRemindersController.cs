using brokenHeart.Auxiliary;
using brokenHeart.Database.DAO.Effects;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.RoundReminders
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundRemindersController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public RoundRemindersController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/RoundReminders
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoundReminder>>> GetRoundReminders()
        {
            if (_context.RoundReminders == null || _context.RoundReminders.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<RoundReminder> roundReminders = FullRoundReminders()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as RoundReminder)
                .ToList();

            return Ok(roundReminders);
        }

        // GET: api/RoundReminders/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RoundReminder>> GetRoundReminder(int id)
        {
            if (_context.RoundReminders == null || _context.RoundReminders.Count() == 0)
            {
                return NotFound();
            }

            RoundReminder roundReminder = ApiAuxiliary.GetEntityPrepare(
                await FullRoundReminders().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (roundReminder == null)
            {
                return NotFound();
            }

            return roundReminder;
        }

        // PATCH: api/RoundReminders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchRoundReminder(
            int id,
            JsonPatchDocument<RoundReminder> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            RoundReminder roundReminder = FullRoundReminders().Single(x => x.Id == id);

            if (roundReminder == null)
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
                    typeof(RoundReminder),
                    roundReminder,
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

        // POST: api/RoundReminders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RoundReminder>> PostRoundReminder(
            RoundReminder roundReminder
        )
        {
            if (_context.RoundReminders == null)
            {
                return Problem("Entity set 'BrokenDbContext.RoundReminders'  is null.");
            }

            RoundReminder returnRoundReminder = ApiAuxiliary.PostEntity(
                _context,
                typeof(RoundReminder),
                roundReminder
            );

            return CreatedAtAction(
                "GetRoundReminder",
                new { id = returnRoundReminder.Id },
                returnRoundReminder
            );
        }

        // DELETE: api/RoundReminders/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRoundReminder(int id)
        {
            if (_context.RoundReminders == null)
            {
                return NotFound();
            }
            var roundReminder = await _context.RoundReminders.FindAsync(id);
            if (roundReminder == null)
            {
                return NotFound();
            }

            _context.RoundReminders.Remove(roundReminder);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<RoundReminder> FullRoundReminders()
        {
            return _context.RoundReminders;
        }
    }
}
