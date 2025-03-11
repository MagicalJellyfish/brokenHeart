using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Projection.Stats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye.Entities.Characters
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class StatController : ControllerBase
    {
        private readonly IStatProjectionService _statProjectionService;

        public StatController(IStatProjectionService statProjectionService)
        {
            _statProjectionService = statProjectionService;
        }

        [HttpGet]
        public ActionResult<List<StatModel>> GetStats()
        {
            List<StatModel> stats = _statProjectionService.GetStats();

            return Ok(stats);
        }
    }
}
