using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.DataTransfer.Save.Entities;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Effects
{
    internal class EffectSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Effect;

        private readonly BrokenDbContext _context;
        private readonly IModifierSaveService _modifierSaveService;
        private readonly IOrderableSaveService _orderableSaveService;

        public EffectSaveService(
            BrokenDbContext context,
            IModifierSaveService modifierSaveService,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _modifierSaveService = modifierSaveService;
            _orderableSaveService = orderableSaveService;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
        {
            Effect effect = new Effect();

            switch (parentType)
            {
                case ElementParentType.Character:
                    effect.CharacterId = parentId;
                    break;
                default:
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Effects.Add(effect);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = effect.Id };
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<Effect>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Effect effect = _context.Effects.Single(x => x.Id == id);

            _modifierSaveService.UpdateGivenModifier(effect, updates);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Effect.Name):
                        effect.Hp = update.Value;
                        break;
                    case nameof(Effect.MaxTempHp):
                        effect.MaxTempHp = update.Value.SafeParseInt();
                        break;
                    case nameof(Effect.Duration):
                        effect.Duration = update.Value;
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.Effects.Remove(_context.Effects.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
