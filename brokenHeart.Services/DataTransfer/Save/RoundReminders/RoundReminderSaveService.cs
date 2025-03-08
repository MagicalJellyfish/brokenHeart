using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.RoundReminders
{
    internal class RoundReminderSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Reminder;

        private readonly BrokenDbContext _context;

        public RoundReminderSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
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
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.RoundReminders.Add(roundReminder);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = roundReminder.Id };
        }

        public void DeleteElement(int id)
        {
            _context.RoundReminders.Remove(_context.RoundReminders.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
