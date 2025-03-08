using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Counters;
using brokenHeart.Services.Utility;

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

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Counter counter = _context.Counters.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch ((CounterField)update.FieldId)
                {
                    case CounterField.Name:
                        counter.Name = update.Value;
                        break;
                    case CounterField.Description:
                        counter.Description = update.Value;
                        break;
                    case CounterField.Value:
                        counter.Value = update.Value.SafeParseInt();
                        break;
                    case CounterField.Max:
                        counter.Max = update.Value.SafeParseInt();
                        break;
                    case CounterField.RoundBased:
                        counter.RoundBased = bool.Parse(update.Value);
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.Counters.Remove(_context.Counters.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
