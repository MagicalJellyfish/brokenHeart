using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye.Entities
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AbilityController : ControllerBase
    {
        private readonly IRollingSaveService _rollingSaveService;

        public AbilityController(IRollingSaveService rollingSaveService)
        {
            _rollingSaveService = rollingSaveService;
        }

        [HttpPut("rolls/{id}")]
        public ActionResult UpdateRolls(int id, List<RollModel> rolls)
        {
            _rollingSaveService.UpdateRolls<Ability>(id, rolls);

            return Ok();
        }
    }
}
