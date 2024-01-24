using Microsoft.AspNetCore.Mvc;
using brokenHeart.DB;
using brokenHeart.Auxiliary;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Operations;
using brokenHeart.Entities.Items;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.Traits;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.Items
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly BrokenDbContext _context;

	    public ItemsController(BrokenDbContext context)
        {
            _context = context;
        }

	    // GET: api/Items
	    [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            if (_context.Items == null || _context.Items.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<Item> items = FullItems().Select(x => ApiAuxiliary.GetEntityPrepare(x) as Item).ToList();

            return Ok(items);
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            if (_context.Items == null || _context.Items.Count() == 0)
            {
                return NotFound();
            }

            Item item = ApiAuxiliary.GetEntityPrepare(await FullItems().FirstOrDefaultAsync(x => x.Id == id));

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        // PATCH: api/Items/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchItem(int id, JsonPatchDocument<Item> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            Item item = FullItems().Single(x => x.Id == id);

            if(item == null)
            {
                return BadRequest();
            }

            List<Operation> operations = new List<Operation>();
            foreach(var operation in patchDocument.Operations)
            {
                operations.Add(operation);
            }

            try
            {
                ApiAuxiliary.PatchEntity(_context, typeof(Item), item, operations);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Items
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            if (_context.Items == null)
            {
              return Problem("Entity set 'BrokenDbContext.Items'  is null.");
            }

            Item returnItem = ApiAuxiliary.PostEntity(_context, typeof(Item), item);

            return CreatedAtAction("GetItem", new { id = returnItem.Id }, returnItem);
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (_context.Items == null)
            {
                return NotFound();
            }
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            _context.SaveChanges();

            return NoContent();
        }

        private IQueryable<Item> FullItems()
        {
            return _context.Items
                .Include(x => x.Counters)
                .Include(x => x.RoundReminder)
                .Include(x => x.StatIncreases).ThenInclude(x => x.Stat);
        }
    }
}
