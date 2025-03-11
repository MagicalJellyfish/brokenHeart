using brokenHeart.Models;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Projection.Characters;
using brokenHeart.Services.DataTransfer.Save.Characters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye.Entities.Characters
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterProjectionService _characterProjectionService;
        private readonly ICharacterSaveService _characterSaveService;

        public CharacterController(
            ICharacterProjectionService characterProjectionService,
            ICharacterSaveService characterSaveService
        )
        {
            _characterProjectionService = characterProjectionService;
            _characterSaveService = characterSaveService;
        }

        [HttpGet("{id}")]
        public ActionResult<CharacterView> GetCharacterView(int id)
        {
            CharacterView? characterView = _characterProjectionService.GetCharacterView(
                new CharacterSearch() { Id = id }
            );

            if (characterView == null)
            {
                return NotFound($"Could not find a character with ID {id}");
            }

            return Ok(characterView);
        }

        [HttpPost]
        public ActionResult<int> CreateCharacter()
        {
            ExecutionResult<int> result = _characterSaveService.CreateCharacter(User.Identity.Name);

            if (!result.Succeeded)
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}/injuries")]
        public ActionResult UpdateInjuries(int id, List<InjuryModel> injuries)
        {
            ExecutionResult result = _characterSaveService.UpdateInjuries(id, injuries);

            if (!result.Succeeded)
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }

            return Ok();
        }

        [HttpPatch("{id}")]
        public ActionResult PatchCharacter(int id, List<CharacterPatch> patches)
        {
            ExecutionResult result = _characterSaveService.PatchCharacter(id, patches);

            if (!result.Succeeded)
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCharacter(int id)
        {
            _characterSaveService.DeleteCharacter(id);

            return Ok();
        }
    }
}
