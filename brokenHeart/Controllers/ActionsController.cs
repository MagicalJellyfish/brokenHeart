using Dice;
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
        public async Task<List<string>> Roll(string rollString)
        {
            return Roller.Roll(rollString).ToString().Split("=>").Select(x => x.Trim()).ToList();
        }
    }
}
