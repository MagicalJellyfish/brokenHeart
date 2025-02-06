using brokenHeart.Database.DAO.Stats;
using brokenHeart.DB;
using brokenHeart.Services.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.StatValues
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatValuesController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly IEndpointEntityService _endpointEntityService;

        public StatValuesController(
            BrokenDbContext context,
            IEndpointEntityService endpointEntityService
        )
        {
            _context = context;
            _endpointEntityService = endpointEntityService;
        }

        // GET: api/StatValues
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<StatValue>>> GetStatValues()
        {
            if (_context.StatValues == null || _context.StatValues.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<StatValue> statValues = FullStatValues()
                .Select(x => _endpointEntityService.GetEntityPrepare(x) as StatValue)
                .ToList();

            return Ok(statValues);
        }

        // GET: api/StatValues/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<StatValue>> GetStatValue(int id)
        {
            if (_context.StatValues == null || _context.StatValues.Count() == 0)
            {
                return NotFound();
            }

            StatValue statValue = _endpointEntityService.GetEntityPrepare(
                await FullStatValues().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (statValue == null)
            {
                return NotFound();
            }

            return statValue;
        }

        // PATCH: api/StatValues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchStatValue(
            int id,
            JsonPatchDocument<StatValue> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            StatValue statValue = FullStatValues().Single(x => x.Id == id);

            if (statValue == null)
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
                    typeof(StatValue),
                    statValue,
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

        // POST: api/StatValues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<StatValue>> PostStatValue(StatValue statValue)
        {
            if (_context.StatValues == null)
            {
                return Problem("Entity set 'BrokenDbContext.StatValues'  is null.");
            }

            StatValue returnStatValue = _endpointEntityService.PostEntity(
                _context,
                typeof(StatValue),
                statValue
            );

            return CreatedAtAction(
                "GetStatValue",
                new { id = returnStatValue.Id },
                returnStatValue
            );
        }

        // DELETE: api/StatValues/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteStatValue(int id)
        {
            if (_context.StatValues == null)
            {
                return NotFound();
            }
            var statValue = await _context.StatValues.FindAsync(id);
            if (statValue == null)
            {
                return NotFound();
            }

            _context.StatValues.Remove(statValue);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<StatValue> FullStatValues()
        {
            return _context.StatValues.Include(x => x.Stat);
        }
    }
}
