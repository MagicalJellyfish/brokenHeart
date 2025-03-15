using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Entities.Modifiers.Traits
{
    internal class TraitTemplateSaveService : IElementSaveService, ITemplateSaveService
    {
        public ElementType SaveType => ElementType.TraitTemplate;

        private readonly BrokenDbContext _context;
        private readonly IModifierTemplateSaveService _modifierTemplateSaveService;
        private readonly IOrderableSaveService _orderableSaveService;

        public TraitTemplateSaveService(
            BrokenDbContext context,
            IModifierTemplateSaveService modifierTemplateSaveService,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _modifierTemplateSaveService = modifierTemplateSaveService;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            TraitTemplate traitTemplate = new TraitTemplate();

            if (parentType != ElementParentType.None)
            {
                Assign(traitTemplate, parentType, (int)parentId);
            }

            _context.TraitTemplates.Add(traitTemplate);
            _context.SaveChanges();

            return traitTemplate.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<TraitTemplate>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            TraitTemplate traitTemplate = _context.TraitTemplates.Single(x => x.Id == id);

            _modifierTemplateSaveService.UpdateGivenModifierTemplate(traitTemplate, updates);

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.TraitTemplates.Remove(_context.TraitTemplates.Single(x => x.Id == id));
            _context.SaveChanges();
        }

        public void RelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            TraitTemplate traitTemplate = _context.TraitTemplates.Single(x => x.Id == id);
            Assign(traitTemplate, parentType, parentId);

            _context.SaveChanges();
        }

        public void UnrelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            TraitTemplate traitTemplate = _context
                .TraitTemplates.Include(x => x.CharacterTemplates)
                .Single(x => x.Id == id);

            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    traitTemplate.CharacterTemplates.Remove(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.SaveChanges();
        }

        public int InstantiateTemplate(int id, ElementParentType parentType, int parentId)
        {
            IQueryable<TraitTemplate> traitTemplate = _context.TraitTemplates.Where(x =>
                x.Id == id
            );

            Trait trait = traitTemplate
                .Select(x => Instantiation.InstantiateTrait.Invoke(x))
                .Single();

            switch (parentType)
            {
                case ElementParentType.Character:
                    trait.CharacterId = parentId;
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.Traits.Add(trait);
            _context.SaveChanges();

            return trait.Id;
        }

        private void Assign(TraitTemplate traitTemplate, ElementParentType parentType, int parentId)
        {
            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    traitTemplate.CharacterTemplates.Add(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }
        }
    }
}
