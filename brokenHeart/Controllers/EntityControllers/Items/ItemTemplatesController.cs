using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Services.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers.EntityControllers.ItemTemplates
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemTemplatesController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly IEndpointEntityService _endpointEntityService;

        public ItemTemplatesController(
            BrokenDbContext context,
            IEndpointEntityService endpointEntityService
        )
        {
            _context = context;
            _endpointEntityService = endpointEntityService;
        }

        // GET: api/ItemTemplates
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ItemTemplate>>> GetItemTemplates()
        {
            if (_context.ItemTemplates == null || _context.ItemTemplates.Count() == 0)
            {
                return NotFound();
            }

            IEnumerable<ItemTemplate> itemTemplates = FullItemTemplates()
                .Select(x => _endpointEntityService.GetEntityPrepare(x) as ItemTemplate)
                .ToList();

            return Ok(itemTemplates);
        }

        // GET: api/ItemTemplates/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ItemTemplate>> GetItemTemplate(int id)
        {
            if (_context.ItemTemplates == null || _context.ItemTemplates.Count() == 0)
            {
                return NotFound();
            }

            ItemTemplate itemTemplate = _endpointEntityService.GetEntityPrepare(
                await FullItemTemplates().FirstOrDefaultAsync(x => x.Id == id)
            );

            if (itemTemplate == null)
            {
                return NotFound();
            }

            return itemTemplate;
        }

        // PATCH: api/ItemTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchItemTemplate(
            int id,
            JsonPatchDocument<ItemTemplate> patchDocument
        )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            ItemTemplate itemTemplate = FullItemTemplates().Single(x => x.Id == id);

            if (itemTemplate == null)
            {
                return BadRequest();
            }

            List<Operation> operations = new List<Operation>();
            foreach (var operation in patchDocument.Operations)
            {
                operations.Add(operation);
            }

            try
            {
                _endpointEntityService.PatchEntity(
                    _context,
                    typeof(ItemTemplate),
                    itemTemplate,
                    operations
                );
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/ItemTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ItemTemplate>> PostItemTemplate(ItemTemplate itemTemplate)
        {
            if (_context.ItemTemplates == null)
            {
                return Problem("Entity set 'BrokenDbContext.ItemTemplates'  is null.");
            }

            ItemTemplate returnItemTemplate = _endpointEntityService.PostEntity(
                _context,
                typeof(ItemTemplate),
                itemTemplate
            );

            return CreatedAtAction(
                "GetItemTemplate",
                new { id = returnItemTemplate.Id },
                returnItemTemplate
            );
        }

        // DELETE: api/ItemTemplates/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteItemTemplate(int id)
        {
            if (_context.ItemTemplates == null)
            {
                return NotFound();
            }
            var itemTemplate = await _context.ItemTemplates.FindAsync(id);
            if (itemTemplate == null)
            {
                return NotFound();
            }

            _context.ItemTemplates.Remove(itemTemplate);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/ItemTemplates/Instantiate/5
        [HttpGet("Instantiate/{id}")]
        [Authorize]
        public async Task<ActionResult<Item>> InstantiateItemTemplate(int id)
        {
            if (_context.ItemTemplates == null || _context.ItemTemplates.Count() == 0)
            {
                return NotFound();
            }

            ItemTemplate itemTemplate = FullItemTemplates().Single(x => x.Id == id);

            if (itemTemplate == null)
            {
                return NotFound();
            }

            Item item = itemTemplate.Instantiate();

            return item;
        }

        private IQueryable<ItemTemplate> FullItemTemplates()
        {
            return _context
                .ItemTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.CounterTemplates)
                .Include(x => x.RoundReminderTemplate)
                .Include(x => x.StatIncreases)
                .ThenInclude(x => x.Stat)
                .Include(x => x.AbilityTemplates);
        }
    }
}
