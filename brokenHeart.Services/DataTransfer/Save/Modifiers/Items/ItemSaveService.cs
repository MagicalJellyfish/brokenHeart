using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Items
{
    internal class ItemSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Item;

        private readonly BrokenDbContext _context;

        public ItemSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
        {
            Item item = new Item();

            switch (parentType)
            {
                case ElementParentType.Character:
                    item.CharacterId = parentId;
                    break;
                default:
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Items.Add(item);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = item.Id };
        }

        public void DeleteElement(int id)
        {
            _context.Items.Remove(_context.Items.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
