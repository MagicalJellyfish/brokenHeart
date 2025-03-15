using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.Utility;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Entities.Modifiers.Items
{
    internal class ItemTemplateSaveService : IElementSaveService, ITemplateSaveService
    {
        public ElementType SaveType => ElementType.ItemTemplate;

        private readonly BrokenDbContext _context;
        private readonly IModifierTemplateSaveService _modifierTemplateSaveService;
        private readonly IOrderableSaveService _orderableSaveService;

        public ItemTemplateSaveService(
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
            ItemTemplate itemTemplate = new ItemTemplate();

            if (parentType != ElementParentType.None)
            {
                Assign(itemTemplate, parentType, (int)parentId);
            }

            _context.ItemTemplates.Add(itemTemplate);
            _context.SaveChanges();

            return itemTemplate.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<ItemTemplate>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            ItemTemplate itemTemplate = _context.ItemTemplates.Single(x => x.Id == id);

            _modifierTemplateSaveService.UpdateGivenModifierTemplate(itemTemplate, updates);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(ItemTemplate.Amount):
                        itemTemplate.Amount = update.Value.SafeParseInt();
                        break;
                    case nameof(ItemTemplate.Unit):
                        itemTemplate.Unit = update.Value;
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.ItemTemplates.Remove(_context.ItemTemplates.Single(x => x.Id == id));
            _context.SaveChanges();
        }

        public void RelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            ItemTemplate itemTemplate = _context.ItemTemplates.Single(x => x.Id == id);
            Assign(itemTemplate, parentType, parentId);

            _context.SaveChanges();
        }

        public void UnrelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            ItemTemplate itemTemplate = _context
                .ItemTemplates.Include(x => x.CharacterTemplates)
                .Single(x => x.Id == id);

            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    itemTemplate.CharacterTemplates.Remove(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.SaveChanges();
        }

        private void Assign(ItemTemplate itemTemplate, ElementParentType parentType, int parentId)
        {
            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    itemTemplate.CharacterTemplates.Add(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }
        }
    }
}
