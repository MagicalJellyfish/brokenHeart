using brokenHeart.Auxiliary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        public ActionsController() { }

        [HttpGet]
        [Route("roll")]
        public async Task<RollResult> Roll(string rollString)
        {
            return RollAuxiliary.RollString(rollString);
        }
    }
}
