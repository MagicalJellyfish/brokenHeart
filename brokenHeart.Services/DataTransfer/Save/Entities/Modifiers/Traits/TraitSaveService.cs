using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.DataTransfer.Save.Entities;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Traits
{
    internal class TraitSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Trait;

        private readonly BrokenDbContext _context;
        private readonly IModifierSaveService _modifierSaveService;
        private readonly IOrderableSaveService _orderableSaveService;

        public TraitSaveService(
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

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<Trait>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Trait trait = _context.Traits.Single(x => x.Id == id);

            _modifierSaveService.UpdateGivenModifier(trait, updates);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Trait.Active):
                        trait.Active = bool.Parse(update.Value);
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.Traits.Remove(_context.Traits.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
