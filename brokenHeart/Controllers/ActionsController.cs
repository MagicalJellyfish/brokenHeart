using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public ActionsController(BrokenDbContext brokenDbContext)
        {
            _context = brokenDbContext;
        }

        [HttpGet("roll")]
        public async Task<RollResult> Roll(string rollString)
        {
            return RollAuxiliary.RollString(rollString);
        }

        [HttpGet("rollChar/{id}")]
        public async Task<ActionResult<RollResult>> RollChar(int id, string rollString)
        {
            Character c = _context.Characters.Include(x => x.Counters).SingleOrDefault(x => x.Id == id);
            if(c == null)
            {
                return NotFound("No character found!");
            }

            return RollAuxiliary.CharRollString(rollString, c);
        }
    }
}
