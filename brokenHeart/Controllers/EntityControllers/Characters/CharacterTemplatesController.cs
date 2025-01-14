using brokenHeart.Auxiliary;
using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Characters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterTemplatesController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public CharacterTemplatesController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/CharacterTemplates/
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CharacterTemplate>>> GetCharacterTemplates()
        {
            if (_context.CharacterTemplates == null || _context.CharacterTemplates.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<CharacterTemplate> characterTemplates = _context
                .CharacterTemplates.Select(x =>
                    ApiAuxiliary.GetEntityPrepare(x) as CharacterTemplate
                )
                .ToList();

            return Ok(characterTemplates);
        }

        // GET: api/CharacterTemplates/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CharacterTemplate>> GetCharacterTemplate(int id)
        {
            if (_context.CharacterTemplates == null || _context.CharacterTemplates.Count() == 0)
            {
                return NotFound();
            }

            CharacterTemplate characterTemplate = ApiAuxiliary.GetEntityPrepare(
                await FullCharacterTemplates().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (characterTemplate == null)
            {
                return NotFound();
            }

            return characterTemplate;
        }

        // PATCH: api/CharacterTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchCharacterTemplates(
            int id,
            JsonPatchDocument<CharacterTemplate> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            CharacterTemplate characterTemplate = FullCharacterTemplates().Single(x => x.Id == id);

            if (characterTemplate == null)
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
                    typeof(CharacterTemplate),
                    characterTemplate,
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

        // POST: api/CharacterTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CharacterTemplate>> PostCharacterTemplate(
            CharacterTemplate characterTemplate
        )
        {
            if (_context.CharacterTemplates == null)
            {
                return Problem("Entity set 'BrokenDbContext.CharacterTemplates'  is null.");
            }

            CharacterTemplate returnCharacterTemplate = ApiAuxiliary.PostEntity(
                _context,
                typeof(CharacterTemplate),
                characterTemplate
            );

            return CreatedAtAction(
                "GetCharacterTemplate",
                new { id = returnCharacterTemplate.Id },
                returnCharacterTemplate
            );
        }

        // DELETE: api/CharacterTemplates/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            if (_context.CharacterTemplates == null)
            {
                return NotFound();
            }
            var characterTemplate = await _context.CharacterTemplates.FindAsync(id);
            if (characterTemplate == null)
            {
                return NotFound();
            }

            _context.CharacterTemplates.Remove(characterTemplate);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/CharacterTemplates/Instantiate/5
        [HttpGet("Instantiate/{id}")]
        [Authorize]
        public async Task<ActionResult<Character>> InstantiateCharacterTemplate(int id)
        {
            if (_context.CharacterTemplates == null || _context.CharacterTemplates.Count() == 0)
            {
                return NotFound();
            }

            CharacterTemplate characterTemplate = FullCharacterTemplates().Single(x => x.Id == id);

            if (characterTemplate == null)
            {
                return NotFound();
            }

            Character character = characterTemplate.Instantiate(
                _context.UserSimplified.Single(x => x.Username == User.Identity.Name)
            );

            return character;
        }

        // csharpier-ignore
        private IQueryable<CharacterTemplate> FullCharacterTemplates()
        {
            return _context.CharacterTemplates
                .Include(x => x.AbilityTemplates).ThenInclude(x => x.AppliedEffectTemplates)
                .Include(x => x.AbilityTemplates).ThenInclude(x => x.Rolls)

                .Include(x => x.RoundReminderTemplates)

                .Include(x => x.Variables)

                .Include(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)

                .Include(x => x.EffectTemplates).ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.EffectTemplates).ThenInclude(x => x.EffectCounterTemplate).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.EffectTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.EffectTemplates).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)
                .Include(x => x.EffectTemplates).ThenInclude(x => x.AbilityTemplates).ThenInclude(x => x.AppliedEffectTemplates)
                .Include(x => x.EffectTemplates).ThenInclude(x => x.AbilityTemplates).ThenInclude(x => x.Rolls)

                .Include(x => x.ItemTemplates).ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.ItemTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.ItemTemplates).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)
                .Include(x => x.ItemTemplates).ThenInclude(x => x.AbilityTemplates).ThenInclude(x => x.AppliedEffectTemplates)
                .Include(x => x.ItemTemplates).ThenInclude(x => x.AbilityTemplates).ThenInclude(x => x.Rolls)

                .Include(x => x.TraitTemplates).ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.TraitTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.TraitTemplates).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)
                .Include(x => x.TraitTemplates).ThenInclude(x => x.AbilityTemplates).ThenInclude(x => x.AppliedEffectTemplates)
                .Include(x => x.TraitTemplates).ThenInclude(x => x.AbilityTemplates).ThenInclude(x => x.Rolls);
        }
    }
}
