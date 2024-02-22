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
    public class AbilityTemplatesController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public AbilityTemplatesController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/AbilityTemplates
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AbilityTemplate>>> GetAbilityTemplates()
        {
            if (_context.AbilityTemplates == null || _context.AbilityTemplates.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<AbilityTemplate> abilityTemplates = FullAbilityTemplates().Select(x => ApiAuxiliary.GetEntityPrepare(x) as AbilityTemplate).ToList();

            return Ok(abilityTemplates);
        }

        // GET: api/AbilityTemplates/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AbilityTemplate>> GetAbilityTemplate(int id)
        {
            if (_context.AbilityTemplates == null || _context.AbilityTemplates.Count() == 0)
            {
                return NotFound();
            }

            AbilityTemplate abilityTemplate = ApiAuxiliary.GetEntityPrepare(await FullAbilityTemplates().FirstOrDefaultAsync(x => x.Id == id));

            if (abilityTemplate == null)
            {
                return NotFound();
            }

            return abilityTemplate;
        }

        // PATCH: api/AbilityTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchAbilityTemplate(int id, JsonPatchDocument<AbilityTemplate> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            AbilityTemplate abilityTemplate = FullAbilityTemplates().Single(x => x.Id == id);

            if (abilityTemplate == null)
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
                ApiAuxiliary.PatchEntity(_context, typeof(AbilityTemplate), abilityTemplate, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/AbilityTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AbilityTemplate>> PostAbilityTemplate(AbilityTemplate abilityTemplate)
        {
            if (_context.AbilityTemplates == null)
            {
                return Problem("Entity set 'BrokenDbContext.AbilityTemplates'  is null.");
            }

            AbilityTemplate returnAbilityTemplate = ApiAuxiliary.PostEntity(_context, typeof(AbilityTemplate), abilityTemplate);

            return CreatedAtAction("GetAbilityTemplate", new { id = returnAbilityTemplate.Id }, returnAbilityTemplate);
        }

        // DELETE: api/AbilityTemplates/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAbilityTemplate(int id)
        {
            if (_context.AbilityTemplates == null)
            {
                return NotFound();
            }
            var abilityTemplate = await _context.AbilityTemplates.FindAsync(id);
            if (abilityTemplate == null)
            {
                return NotFound();
            }

            _context.AbilityTemplates.Remove(abilityTemplate);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/AbilityTemplates/Instantiate/5
        [HttpGet("Instantiate/{id}")]
        [Authorize]
        public async Task<ActionResult<Ability>> InstantiateAbilityTemplate(int id)
        {
            if (_context.AbilityTemplates == null || _context.AbilityTemplates.Count() == 0)
            {
                return NotFound();
            }

            AbilityTemplate abilityTemplate = await _context.AbilityTemplates.FindAsync(id);

            if (abilityTemplate == null)
            {
                return NotFound();
            }

            Ability ability = abilityTemplate.Instantiate();

            return ability;
        }

        private IQueryable<AbilityTemplate> FullAbilityTemplates()
        {
            return _context.AbilityTemplates
                .Include(x => x.Rolls)
                .Include(x => x.EffectTemplates)
                .Include(x => x.CharacterTemplates);
        }
    }
}
