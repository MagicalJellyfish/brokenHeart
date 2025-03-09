using brokenHeart.Models.DataTransfer;
using brokenHeart.Services.DataTransfer.Save.Abilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye.Entities
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AbilityController : ControllerBase
    {
        private readonly IAbilitySaveService _abilitySaveService;

        public AbilityController(IAbilitySaveService abilitySaveService)
        {
            _abilitySaveService = abilitySaveService;
        }

        [HttpPut("rolls/{id}")]
        public ActionResult UpdateRolls(int id, List<RollModel> rolls)
        {
            _abilitySaveService.UpdateRolls(id, rolls);

            return Ok();
        }
    }
}
