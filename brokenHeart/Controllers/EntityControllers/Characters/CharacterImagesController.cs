using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities.Characters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Controllers.EntityControllers.Characters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterImagesController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public CharacterImagesController(BrokenDbContext context)
        {
            _context = context;
        }

        // PATCH: api/CharacterImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchCharacterImage(
            int id,
            JsonPatchDocument<CharacterImage> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            CharacterImage characterImage = _context.CharacterImages.Single(x => x.Id == id);

            if (characterImage == null)
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
                    typeof(CharacterImage),
                    characterImage,
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

        // DELETE: api/CharacterImages/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCharacterImage(int id)
        {
            if (_context.CharacterImages == null)
            {
                return NotFound();
            }
            var characterImage = await _context.CharacterImages.FindAsync(id);
            if (characterImage == null)
            {
                return NotFound();
            }

            _context.CharacterImages.Remove(characterImage);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
