using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Modifiers.Effects;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Effects
{
    internal class EffectSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Effect;

        private readonly BrokenDbContext _context;
        private readonly IModifierSaveService _modifierSaveService;

        public EffectSaveService(BrokenDbContext context, IModifierSaveService modifierSaveService)
        {
            _context = context;
            _modifierSaveService = modifierSaveService;
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

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Effect effect = _context.Effects.Single(x => x.Id == id);

            _modifierSaveService.UpdateGivenModifier(effect, updates);

            foreach (ElementUpdate update in updates)
            {
                switch ((EffectField)update.FieldId)
                {
                    case EffectField.Hp:
                        effect.Hp = update.Value;
                        break;
                    case EffectField.MaxTempHp:
                        effect.MaxTempHp = update.Value.SafeParseInt();
                        break;
                    case EffectField.Duration:
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
