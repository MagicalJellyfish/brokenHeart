using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities.Abilities.Abilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Abilities
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbilitiesController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public AbilitiesController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/Abilities
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Ability>>> GetAbilities()
        {
            if (_context.Abilities == null || _context.Abilities.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Ability> abilities = FullAbilities()
                .Select(x => ApiAuxiliary.GetEntityPrepare(x) as Ability)
                .ToList();

            return Ok(abilities);
        }

        // GET: api/Abilities/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Ability>> GetAbility(int id)
        {
            if (_context.Abilities == null || _context.Abilities.Count() == 0)
            {
                return NotFound();
            }

            Ability ability = ApiAuxiliary.GetEntityPrepare(
                await FullAbilities().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (ability == null)
            {
                return NotFound();
            }

            return ability;
        }

        // PATCH: api/Abilities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchAbility(
            int id,
            JsonPatchDocument<Ability> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Ability ability = FullAbilities().Single(x => x.Id == id);

            if (ability == null)
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
                ApiAuxiliary.PatchEntity(_context, typeof(Ability), ability, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Abilities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Ability>> PostAbility(Ability ability)
        {
            if (_context.Abilities == null)
            {
                return Problem("Entity set 'BrokenDbContext.Abilities'  is null.");
            }

            Ability returnAbility = ApiAuxiliary.PostEntity(_context, typeof(Ability), ability);

            return CreatedAtAction("GetAbility", new { id = returnAbility.Id }, returnAbility);
        }

        // DELETE: api/Abilities/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAbility(int id)
        {
            if (_context.Abilities == null)
            {
                return NotFound();
            }
            var ability = await _context.Abilities.FindAsync(id);
            if (ability == null)
            {
                return NotFound();
            }

            _context.Abilities.Remove(ability);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<Ability> FullAbilities()
        {
            return _context
                .Abilities.Include(x => x.Rolls)
                .Include(x => x.EffectTemplates)
                .Include(x => x.Character);
        }
    }
}
