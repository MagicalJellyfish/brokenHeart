using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Abilities
{
    internal class AbilitySaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Ability;

        private readonly BrokenDbContext _context;

        public AbilitySaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
        {
            Ability ability = new Ability();

            switch (parentType)
            {
                case ElementParentType.Character:
                    ability.CharacterId = parentId;
                    break;
                case ElementParentType.Modifier:
                    ability.ModifierId = parentId;
                    break;
                default:
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Abilities.Add(ability);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = ability.Id };
        }

        public void DeleteElement(int id)
        {
            _context.Abilities.Remove(_context.Abilities.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
