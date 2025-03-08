using brokenHeart.Database.DAO.Characters;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Characters
{
    internal class VariableSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Variable;

        private readonly BrokenDbContext _context;

        public VariableSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
        {
            Variable variable = new Variable();

            switch (parentType)
            {
                case ElementParentType.Character:
                    variable.CharacterId = parentId;
                    break;
                default:
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Variables.Add(variable);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = variable.Id };
        }

        public void DeleteElement(int id)
        {
            _context.Variables.Remove(_context.Variables.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
