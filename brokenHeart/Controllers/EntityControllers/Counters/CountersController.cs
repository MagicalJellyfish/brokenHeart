using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Services.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Counters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountersController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly IEndpointEntityService _endpointEntityService;

        public CountersController(
            BrokenDbContext context,
            IEndpointEntityService endpointEntityService
        )
        {
            _context = context;
            _endpointEntityService = endpointEntityService;
        }

        // GET: api/Counters
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Counter>>> GetCounters()
        {
            if (_context.Counters == null || _context.Counters.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Counter> counters = FullCounters()
                .Select(x => _endpointEntityService.GetEntityPrepare(x) as Counter)
                .ToList();

            return Ok(counters);
        }

        // GET: api/Counters/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Counter>> GetCounter(int id)
        {
            if (_context.Counters == null || _context.Counters.Count() == 0)
            {
                return NotFound();
            }
            Counter counter = _endpointEntityService.GetEntityPrepare(
                await FullCounters().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (counter == null)
            {
                return NotFound();
            }

            return counter;
        }

        // PATCH: api/Counters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchCounter(
            int id,
            JsonPatchDocument<Counter> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Counter counter = FullCounters().Single(x => x.Id == id);

            if (counter == null)
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
                _endpointEntityService.PatchEntity(_context, typeof(Counter), counter, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Counters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Counter>> PostCounter(Counter counter)
        {
            if (_context.Counters == null)
            {
                return Problem("Entity set 'BrokenDbContext.Counters'  is null.");
            }

            Counter returnCounter = _endpointEntityService.PostEntity(
                _context,
                typeof(Counter),
                counter
            );

            return CreatedAtAction("GetCounter", new { id = returnCounter.Id }, returnCounter);
        }

        // DELETE: api/Counters/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCounter(int id)
        {
            if (_context.Counters == null)
            {
                return NotFound();
            }
            var counter = await _context.Counters.FindAsync(id);
            if (counter == null)
            {
                return NotFound();
            }

            _context.Counters.Remove(counter);

            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<Counter> FullCounters()
        {
            return _context.Counters.Include(x => x.RoundReminder);
        }
    }
}
