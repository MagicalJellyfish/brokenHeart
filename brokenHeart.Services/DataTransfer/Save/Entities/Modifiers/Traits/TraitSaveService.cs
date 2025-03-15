using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;

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

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            Trait trait = new Trait();

            switch (parentType)
            {
                case ElementParentType.Character:
                    trait.CharacterId = (int)parentId;
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.Traits.Add(trait);
            _context.SaveChanges();

            return trait.Id;
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
