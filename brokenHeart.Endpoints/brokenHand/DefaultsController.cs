using brokenHeart.Services;
using brokenHeart.Services.DataTransfer.Save.Entities;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultsController : ControllerBase
    {
        private readonly IUserSimplifiedSaveService _userSimplifiedSaveService;

        public DefaultsController(IUserSimplifiedSaveService userSimplifiedSaveService)
        {
            _userSimplifiedSaveService = userSimplifiedSaveService;
        }

        [HttpPatch("character")]
        [Localhost]
        public async Task<ActionResult<string>> DefaultCharacter(ulong discordId, int charId)
        {
            string characterName = _userSimplifiedSaveService.UpdateDefaultCharacterAndReturnName(
                discordId,
                charId
            );

            return characterName;
        }

        [HttpPatch("ability")]
        [Localhost]
        public async Task<ActionResult> DefaultAbility(ulong discordId, string shortcut)
        {
            _userSimplifiedSaveService.UpdateDefaultAbility(discordId, shortcut);
            return Ok();
        }

        [HttpPatch("targets")]
        [Localhost]
        public async Task<ActionResult<string>> DefaultTarget(ulong discordId, string targets)
        {
            _userSimplifiedSaveService.UpdateDefaultTarget(discordId, targets);
            return Ok();
        }
    }
}
