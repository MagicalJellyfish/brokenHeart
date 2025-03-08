using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search.Modifiers;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal class ItemSearchService : SearchService, IItemSearchService
    {
        public ItemSearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<Item> GetItems(ItemSearch search)
        {
            IQueryable<Item> items = _context.Items;

            if (search.Id != null)
            {
                items = items.Where(x => x.Id == search.Id);
            }

            return items;
        }
    }
}
