using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities;
using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Stats;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Controllers.EntityControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstantsController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public ConstantsController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/Constants/Stats
        [Route("Stats")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stat>>> GetStats()
        {
            if (_context.Stats == null || _context.Stats.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Stat> stats = _context.Stats.Select(x => ApiAuxiliary.GetEntityPrepare(x) as Stat).ToList();

            return Ok(stats);
        }

        // GET: api/Constants/Bodyparts
        [Route("Bodyparts")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bodypart>>> GetBodyparts()
        {
            if (_context.Bodyparts == null || _context.Bodyparts.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Bodypart> bodyparts = _context.Bodyparts.Select(x => ApiAuxiliary.GetEntityPrepare(x) as Bodypart).ToList();

            return Ok(bodyparts);
        }
    }
}
