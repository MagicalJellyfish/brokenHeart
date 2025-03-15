using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;

namespace brokenHeart.Services.DataTransfer.Save.RoundReminders
{
    internal class RoundReminderSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Reminder;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public RoundReminderSaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            RoundReminder roundReminder = new RoundReminder();

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

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<RoundReminder>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            RoundReminder roundReminder = _context.RoundReminders.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(RoundReminder.Reminding):
                        roundReminder.Reminding = bool.Parse(update.Value);
                        break;
                    case nameof(RoundReminder.Reminder):
                        roundReminder.Reminder = update.Value;
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.RoundReminders.Remove(_context.RoundReminders.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
