using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Projection.Characters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye.Entities.Characters
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CharacterListController : ControllerBase
    {
        private readonly ICharacterProjectionService _characterProjectionService;

        public CharacterListController(ICharacterProjectionService characterProjectionService)
        {
            _characterProjectionService = characterProjectionService;
        }

        [HttpQuery("simple")]
        public ActionResult<List<SimpleCharacter>> GetSimpleCharacters(CharacterSearch search)
        {
            return Ok(_characterProjectionService.GetSimpleCharacters(search));
        }
    }
}
