using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Traits
{
    internal class TraitSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Trait;

        private readonly BrokenDbContext _context;

        public TraitSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
        {
            Trait trait = new Trait();

            switch (parentType)
            {
                case ElementParentType.Character:
                    trait.CharacterId = parentId;
                    break;
                default:
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Traits.Add(trait);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = trait.Id };
        }

        public void DeleteElement(int id)
        {
            _context.Traits.Remove(_context.Traits.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
