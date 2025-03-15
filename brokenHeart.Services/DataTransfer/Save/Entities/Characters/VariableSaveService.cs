using brokenHeart.Database.DAO.Characters;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Characters
{
    internal class VariableSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Variable;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public VariableSaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            Variable variable = new Variable();

            switch (parentType)
            {
                case ElementParentType.Character:
                    variable.CharacterId = parentId;
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.Variables.Add(variable);
            _context.SaveChanges();

            return variable.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<Variable>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Variable variable = _context.Variables.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Variable.Name):
                        variable.Name = update.Value;
                        break;
                    case nameof(Variable.Value):
                        variable.Value = update.Value.SafeParseInt();
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.Variables.Remove(_context.Variables.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
