using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search.Characters;
using brokenHeart.Services.DataTransfer.Projection.Characters;
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

        public CharacterController(ICharacterProjectionService characterProjectionService)
        {
            _characterProjectionService = characterProjectionService;
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
    }
}
