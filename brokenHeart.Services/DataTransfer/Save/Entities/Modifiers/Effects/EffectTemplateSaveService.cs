using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.Utility;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Entities.Modifiers.Effects
{
    internal class EffectTemplateSaveService : IElementSaveService, ITemplateSaveService
    {
        public ElementType SaveType => ElementType.EffectTemplate;

        private readonly BrokenDbContext _context;
        private readonly IModifierTemplateSaveService _modifierTemplateSaveService;
        private readonly IOrderableSaveService _orderableSaveService;

        public EffectTemplateSaveService(
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
            EffectTemplate effectTemplate = new EffectTemplate();

            if (parentType != ElementParentType.None)
            {
                Assign(effectTemplate, parentType, (int)parentId);
            }

            _context.EffectTemplates.Add(effectTemplate);
            _context.SaveChanges();

            return effectTemplate.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<EffectTemplate>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            EffectTemplate effectTemplate = _context.EffectTemplates.Single(x => x.Id == id);

            _modifierTemplateSaveService.UpdateGivenModifierTemplate(effectTemplate, updates);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(EffectTemplate.Hp):
                        effectTemplate.Hp = update.Value;
                        break;
                    case nameof(EffectTemplate.MaxTempHp):
                        effectTemplate.MaxTempHp = update.Value.SafeParseInt();
                        break;
                    case nameof(EffectTemplate.Duration):
                        effectTemplate.Duration = update.Value;
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.EffectTemplates.Remove(_context.EffectTemplates.Single(x => x.Id == id));
            _context.SaveChanges();
        }

        public void RelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            EffectTemplate effectTemplate = _context.EffectTemplates.Single(x => x.Id == id);
            Assign(effectTemplate, parentType, parentId);

            _context.SaveChanges();
        }

        public void UnrelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            EffectTemplate effectTemplate = _context
                .EffectTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.ApplyingAbilityTemplates)
                .Single(x => x.Id == id);

            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    effectTemplate.CharacterTemplates.Remove(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.AbilityTemplate:
                    effectTemplate.ApplyingAbilityTemplates.Remove(
                        _context.AbilityTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.SaveChanges();
        }

        private void Assign(
            EffectTemplate effectTemplate,
            ElementParentType parentType,
            int parentId
        )
        {
            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    effectTemplate.CharacterTemplates.Add(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.AbilityTemplate:
                    effectTemplate.ApplyingAbilityTemplates.Add(
                        _context.AbilityTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }
        }
    }
}
