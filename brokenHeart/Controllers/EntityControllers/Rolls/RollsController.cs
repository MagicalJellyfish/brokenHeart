using brokenHeart.Auxiliary;
using brokenHeart.Database.DAO.Abilities;
using brokenHeart.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Rolls
{
    [Route("api/[controller]")]
    [ApiController]
    public class RollsController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public RollsController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/Rolls
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Roll>>> GetRolls()
        {
            if (_context.Rolls == null || _context.Rolls.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Roll> rolls = FullRolls()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as Roll)
                .ToList();

            return Ok(rolls);
        }

        // GET: api/Rolls/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Roll>> GetRoll(int id)
        {
            if (_context.Rolls == null || _context.Rolls.Count() == 0)
            {
                return NotFound();
            }

            Roll roll = ApiAuxiliary.GetEntityPrepare(
                await FullRolls().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (roll == null)
            {
                return NotFound();
            }

            return roll;
        }

        // PATCH: api/Rolls/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchRoll(int id, JsonPatchDocument<Roll> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Roll roll = FullRolls().Single(x => x.Id == id);

            if (roll == null)
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
                ApiAuxiliary.PatchEntity(_context, typeof(Roll), roll, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Rolls
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Roll>> PostRoll(Roll roll)
        {
            if (_context.Rolls == null)
            {
                return Problem("Entity set 'BrokenDbContext.Rolls'  is null.");
            }

            Roll returnRoll = ApiAuxiliary.PostEntity(_context, typeof(Roll), roll);

            return CreatedAtAction("GetRoll", new { id = returnRoll.Id }, returnRoll);
        }

        // DELETE: api/Rolls/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRoll(int id)
        {
            if (_context.Rolls == null)
            {
                return NotFound();
            }
            var roll = await _context.Rolls.FindAsync(id);
            if (roll == null)
            {
                return NotFound();
            }

            _context.Rolls.Remove(roll);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<Roll> FullRolls()
        {
            return _context.Rolls;
        }
    }
}
