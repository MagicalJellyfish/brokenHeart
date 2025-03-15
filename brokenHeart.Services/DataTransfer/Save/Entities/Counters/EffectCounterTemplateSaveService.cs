using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.Utility;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Entities.Counters
{
    internal class EffectCounterTemplateSaveService : IElementSaveService, ITemplateSaveService
    {
        public ElementType SaveType => ElementType.EffectCounterTemplate;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public EffectCounterTemplateSaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            EffectCounterTemplate effectCounterTemplate = new EffectCounterTemplate();

            Assign(effectCounterTemplate, parentType, (int)parentId);

            _context.EffectCounterTemplates.Add(effectCounterTemplate);
            _context.SaveChanges();

            return effectCounterTemplate.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<EffectCounterTemplate>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            EffectCounterTemplate effectCounterTemplate = _context.EffectCounterTemplates.Single(
                x => x.Id == id
            );

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(EffectCounterTemplate.Name):
                        effectCounterTemplate.Name = update.Value;
                        break;
                    case nameof(EffectCounterTemplate.Description):
                        effectCounterTemplate.Description = update.Value;
                        break;
                    case nameof(EffectCounterTemplate.Max):
                        effectCounterTemplate.Max = update.Value.SafeParseInt();
                        break;
                    case nameof(EffectCounterTemplate.RoundBased):
                        effectCounterTemplate.RoundBased = bool.Parse(update.Value);
                        break;
                    case nameof(EffectCounterTemplate.EndEffect):
                        effectCounterTemplate.EndEffect = bool.Parse(update.Value);
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.EffectCounterTemplates.Remove(
                _context.EffectCounterTemplates.Single(x => x.Id == id)
            );
            _context.SaveChanges();
        }

        public void RelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            EffectCounterTemplate effectCounterTemplate = _context.EffectCounterTemplates.Single(
                x => x.Id == id
            );
            Assign(effectCounterTemplate, parentType, parentId);

            _context.SaveChanges();
        }

        public void UnrelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            EffectCounterTemplate effectCounterTemplate = _context
                .EffectCounterTemplates.Include(x => x.EffectTemplates)
                .Single(x => x.Id == id);

            switch (parentType)
            {
                case ElementParentType.ModifierTemplate:
                    effectCounterTemplate.EffectTemplates.Remove(
                        _context.EffectTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.SaveChanges();
        }

        public int InstantiateTemplate(int id, ElementParentType parentType, int parentId)
        {
            IQueryable<EffectCounterTemplate> effectCounterTemplate =
                _context.EffectCounterTemplates.Where(x => x.Id == id);

            EffectCounter effectCounter = effectCounterTemplate
                .Select(x => Instantiation.InstantiateEffectCounter.Invoke(x))
                .Single();

            switch (parentType)
            {
                case ElementParentType.Modifier:
                    effectCounter.EffectId = parentId;
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.EffectCounters.Add(effectCounter);
            _context.SaveChanges();

            return effectCounter.Id;
        }

        private void Assign(
            EffectCounterTemplate effectCounterTemplate,
            ElementParentType parentType,
            int parentId
        )
        {
            switch (parentType)
            {
                case ElementParentType.ModifierTemplate:
                    effectCounterTemplate.EffectTemplates.Add(
                        _context.EffectTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }
        }
    }
}
