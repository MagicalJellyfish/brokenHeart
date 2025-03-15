using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using LinqKit;

namespace brokenHeart.Services.DataTransfer.Save.Entities.RoundReminders
{
    internal class RoundReminderTemplateSaveService : IElementSaveService, ITemplateSaveService
    {
        public ElementType SaveType => ElementType.ReminderTemplate;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public RoundReminderTemplateSaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            RoundReminderTemplate roundReminderTemplate = new RoundReminderTemplate();

            if (parentType != ElementParentType.None)
            {
                Assign(roundReminderTemplate, parentType, (int)parentId);
            }

            _context.RoundReminderTemplates.Add(roundReminderTemplate);
            _context.SaveChanges();

            return roundReminderTemplate.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<RoundReminderTemplate>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            RoundReminderTemplate roundReminderTemplate = _context.RoundReminderTemplates.Single(
                x => x.Id == id
            );

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(RoundReminderTemplate.Reminding):
                        roundReminderTemplate.Reminding = bool.Parse(update.Value);
                        break;
                    case nameof(RoundReminderTemplate.Reminder):
                        roundReminderTemplate.Reminder = update.Value;
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.RoundReminderTemplates.Remove(
                _context.RoundReminderTemplates.Single(x => x.Id == id)
            );
            _context.SaveChanges();
        }

        public void RelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            RoundReminderTemplate roundReminderTemplate = _context.RoundReminderTemplates.Single(
                x => x.Id == id
            );
            Assign(roundReminderTemplate, parentType, parentId);

            _context.SaveChanges();
        }

        public void UnrelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            RoundReminderTemplate roundReminderTemplate = _context.RoundReminderTemplates.Single(
                x => x.Id == id
            );

            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    roundReminderTemplate.CharacterTemplates.Remove(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.ModifierTemplate:
                    roundReminderTemplate.ModifierTemplates.Remove(
                        _context.ModifierTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.CounterTemplate:
                    roundReminderTemplate.CounterTemplates.Remove(
                        _context.CounterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.SaveChanges();
        }

        public int InstantiateTemplate(int id, ElementParentType parentType, int parentId)
        {
            IQueryable<RoundReminderTemplate> roundReminderTemplate =
                _context.RoundReminderTemplates.Where(x => x.Id == id);

            RoundReminder roundReminder = roundReminderTemplate
                .Select(x => Instantiation.InstantiateRoundReminder.Invoke(x)!)
                .Single();

            switch (parentType)
            {
                case ElementParentType.Character:
                    roundReminder.CharacterId = parentId;
                    break;
                case ElementParentType.Modifier:
                    roundReminder.ModifierId = parentId;
                    break;
                case ElementParentType.Counter:
                    roundReminder.CounterId = parentId;
                    break;

                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.RoundReminders.Add(roundReminder);
            _context.SaveChanges();

            return roundReminder.Id;
        }

        private void Assign(
            RoundReminderTemplate roundReminderTemplate,
            ElementParentType parentType,
            int parentId
        )
        {
            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    roundReminderTemplate.CharacterTemplates.Add(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.ModifierTemplate:
                    roundReminderTemplate.ModifierTemplates.Add(
                        _context.ModifierTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.CounterTemplate:
                    roundReminderTemplate.CounterTemplates.Add(
                        _context.CounterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }
        }
    }
}
