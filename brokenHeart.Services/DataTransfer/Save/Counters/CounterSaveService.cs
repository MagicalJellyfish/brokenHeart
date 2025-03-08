using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Counters
{
    internal class CounterSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Counter;

        private readonly BrokenDbContext _context;

        public CounterSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
        {
            Counter counter = new Counter();

            switch (parentType)
            {
                case ElementParentType.Character:
                    counter.CharacterId = parentId;
                    break;
                case ElementParentType.Modifier:
                    counter.ModifierId = parentId;
                    break;
                default:
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Counters.Add(counter);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = counter.Id };
        }

        public void DeleteElement(int id)
        {
            _context.Counters.Remove(_context.Counters.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
