using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Effects
{
    internal class EffectSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Effect;

        private readonly BrokenDbContext _context;

        public EffectSaveService(BrokenDbContext context)
        {
            _context = context;
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

        public void DeleteElement(int id)
        {
            _context.Effects.Remove(_context.Effects.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
