using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.DataTransfer.Save.Entities;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Counters
{
    internal class EffectCounterSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.EffectCounter;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public EffectCounterSaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
        {
            EffectCounter effectCounter = new EffectCounter();

            switch (parentType)
            {
                case ElementParentType.Modifier:
                    effectCounter.EffectId = parentId;
                    break;
                default:
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Counters.Add(effectCounter);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = effectCounter.Id };
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<EffectCounter>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            EffectCounter effectCounter = _context.EffectCounters.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Counter.Name):
                        effectCounter.Name = update.Value;
                        break;
                    case nameof(Counter.Description):
                        effectCounter.Description = update.Value;
                        break;
                    case nameof(Counter.Value):
                        effectCounter.Value = update.Value.SafeParseInt();
                        break;
                    case nameof(Counter.Max):
                        effectCounter.Max = update.Value.SafeParseInt();
                        break;
                    case nameof(Counter.RoundBased):
                        effectCounter.RoundBased = bool.Parse(update.Value);
                        break;
                    case nameof(EffectCounter.EndEffect):
                        effectCounter.EndEffect = bool.Parse(update.Value);
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.EffectCounters.Remove(_context.EffectCounters.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
