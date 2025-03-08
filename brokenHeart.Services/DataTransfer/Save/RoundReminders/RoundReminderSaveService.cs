﻿using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Models.DataTransfer.Save.ElementFields.RoundReminders;

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

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            RoundReminder roundReminder = _context.RoundReminders.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch ((RoundReminderField)update.FieldId)
                {
                    case RoundReminderField.Reminding:
                        roundReminder.Reminding = bool.Parse(update.Value);
                        break;
                    case RoundReminderField.Reminder:
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
