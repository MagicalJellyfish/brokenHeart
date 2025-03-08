using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Models.DataTransfer.Search.Modifiers;

namespace brokenHeart.Services.DataTransfer.Search.Abilities
{
    internal interface IItemSearchService
    {
        public IQueryable<Item> GetItems(ItemSearch search);
    }
}
