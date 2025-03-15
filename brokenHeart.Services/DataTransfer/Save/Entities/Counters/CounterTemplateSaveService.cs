using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.Utility;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Entities.Counters
{
    internal class CounterTemplateSaveService : IElementSaveService, ITemplateSaveService
    {
        public ElementType SaveType => ElementType.CounterTemplate;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public CounterTemplateSaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            CounterTemplate counterTemplate = new CounterTemplate();

            if (parentType != ElementParentType.None)
            {
                Assign(counterTemplate, parentType, (int)parentId);
            }

            _context.CounterTemplates.Add(counterTemplate);
            _context.SaveChanges();

            return counterTemplate.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<CounterTemplate>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            CounterTemplate counterTemplate = _context.CounterTemplates.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(CounterTemplate.Name):
                        counterTemplate.Name = update.Value;
                        break;
                    case nameof(CounterTemplate.Description):
                        counterTemplate.Description = update.Value;
                        break;
                    case nameof(CounterTemplate.Max):
                        counterTemplate.Max = update.Value.SafeParseInt();
                        break;
                    case nameof(CounterTemplate.RoundBased):
                        counterTemplate.RoundBased = bool.Parse(update.Value);
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.CounterTemplates.Remove(_context.CounterTemplates.Single(x => x.Id == id));
            _context.SaveChanges();
        }

        public void RelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            CounterTemplate counterTemplate = _context.CounterTemplates.Single(x => x.Id == id);
            Assign(counterTemplate, parentType, parentId);

            _context.SaveChanges();
        }

        public void UnrelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            CounterTemplate counterTemplate = _context
                .CounterTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.ModifierTemplates)
                .Single(x => x.Id == id);

            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    counterTemplate.CharacterTemplates.Remove(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.ModifierTemplate:
                    counterTemplate.ModifierTemplates.Remove(
                        _context.ModifierTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.SaveChanges();
        }

        private void Assign(
            CounterTemplate counterTemplate,
            ElementParentType parentType,
            int parentId
        )
        {
            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    counterTemplate.CharacterTemplates.Add(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.ModifierTemplate:
                    counterTemplate.ModifierTemplates.Add(
                        _context.ModifierTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }
        }
    }
}
