using brokenHeart.Models.DataTransfer;
using brokenHeart.Services.DataTransfer.Save.Modifiers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye.Entities
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ModifierController : ControllerBase
    {
        private readonly IModifierSaveService _modifierSaveService;

        public ModifierController(IModifierSaveService modifierSaveService)
        {
            _modifierSaveService = modifierSaveService;
        }

        [HttpPut("stats/{id}")]
        public ActionResult UpdateStats(int id, List<StatValueModel> stats)
        {
            _modifierSaveService.UpdateStats(id, stats);

            return Ok();
        }
    }
}
