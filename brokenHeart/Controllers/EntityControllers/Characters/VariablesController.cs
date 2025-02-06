using brokenHeart.Database.DAO.Characters;
using brokenHeart.DB;
using brokenHeart.Services.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Characters
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariablesController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly IEndpointEntityService _endpointEntityService;

        public VariablesController(
            BrokenDbContext context,
            IEndpointEntityService endpointEntityService
        )
        {
            _context = context;
            _endpointEntityService = endpointEntityService;
        }

        // GET: api/Variables
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Variable>>> GetVariables()
        {
            if (_context.Variables == null || _context.Variables.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Variable> variables = FullVariables()
                .Select(x => _endpointEntityService.GetEntityPrepare(x) as Variable)
                .ToList();

            return Ok(variables);
        }

        // GET: api/Variables/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Variable>> GetVariable(int id)
        {
            if (_context.Variables == null || _context.Variables.Count() == 0)
            {
                return NotFound();
            }

            Variable variable = _endpointEntityService.GetEntityPrepare(
                await FullVariables().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (variable == null)
            {
                return NotFound();
            }

            return variable;
        }

        // PATCH: api/Variable/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchVariable(
            int id,
            JsonPatchDocument<Variable> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Variable variable = FullVariables().Single(x => x.Id == id);

            if (variable == null)
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
                    typeof(Variable),
                    variable,
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

        // POST: api/Variables
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Variable>> PostVariable(Variable variable)
        {
            if (_context.Variables == null)
            {
                return Problem("Entity set 'BrokenDbContext.Variables'  is null.");
            }

            Variable returnVariable = _endpointEntityService.PostEntity(
                _context,
                typeof(Variable),
                variable
            );

            return CreatedAtAction("GetVariable", new { id = returnVariable.Id }, returnVariable);
        }

        // DELETE: api/Variables/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVariable(int id)
        {
            if (_context.Variables == null)
            {
                return NotFound();
            }
            var variable = await _context.Variables.FindAsync(id);
            if (variable == null)
            {
                return NotFound();
            }

            _context.Variables.Remove(variable);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<Variable> FullVariables()
        {
            return _context.Variables;
        }
    }
}
