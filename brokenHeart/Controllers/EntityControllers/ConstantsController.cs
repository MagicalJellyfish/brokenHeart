using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.DB;
using brokenHeart.Services.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Controllers.EntityControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstantsController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly IEndpointEntityService _endpointEntityService;

        public ConstantsController(
            BrokenDbContext context,
            IEndpointEntityService endpointEntityService
        )
        {
            _context = context;
            _endpointEntityService = endpointEntityService;
        }

        // GET: api/Constants/Stats
        [HttpGet("Stats")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Stat>>> GetStats()
        {
            if (_context.Stats == null || _context.Stats.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Stat> stats = _context
                .Stats.Select(x => _endpointEntityService.GetEntityPrepare(x) as Stat)
                .ToList();

            return Ok(stats);
        }

        // GET: api/Constants/Bodyparts
        [HttpGet("Bodyparts")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Bodypart>>> GetBodyparts()
        {
            if (_context.Bodyparts == null || _context.Bodyparts.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Bodypart> bodyparts = _context
                .Bodyparts.Select(x => _endpointEntityService.GetEntityPrepare(x) as Bodypart)
                .ToList();

            return Ok(bodyparts);
        }
    }
}
