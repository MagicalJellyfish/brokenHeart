using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Modifiers.Traits;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Traits
{
    internal class TraitSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Trait;

        private readonly BrokenDbContext _context;
        private readonly IModifierSaveService _modifierSaveService;

        public TraitSaveService(BrokenDbContext context, IModifierSaveService modifierSaveService)
        {
            _context = context;
            _modifierSaveService = modifierSaveService;
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
            List<Trait> traits = _context
                .Traits.Where(x => reorders.Select(y => y.Id).Contains(x.Id))
                .ToList();

            foreach (Trait trait in traits)
            {
                trait.ViewPosition = reorders.Single(x => x.Id == trait.Id).ViewPosition;
            }

            _context.SaveChanges();
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Trait trait = _context.Traits.Single(x => x.Id == id);

            _modifierSaveService.UpdateGivenModifier(trait, updates);

            foreach (ElementUpdate update in updates)
            {
                switch ((TraitField)update.FieldId)
                {
                    case TraitField.Active:
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
