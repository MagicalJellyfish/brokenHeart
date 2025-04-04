﻿using brokenHeart.Database.DAO.Counters;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Counters
{
    internal class CounterSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Counter;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public CounterSaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            Counter counter = new Counter();

            switch (parentType)
            {
                case ElementParentType.Character:
                    counter.CharacterId = parentId;
                    break;
                case ElementParentType.Modifier:
                    counter.ModifierId = parentId;
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.Counters.Add(counter);
            _context.SaveChanges();

            return counter.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<Counter>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Counter counter = _context.Counters.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Counter.Name):
                        counter.Name = update.Value;
                        break;
                    case nameof(Counter.Description):
                        counter.Description = update.Value;
                        break;
                    case nameof(Counter.Value):
                        counter.Value = update.Value.SafeParseInt();
                        break;
                    case nameof(Counter.Max):
                        counter.Max = update.Value.SafeParseInt();
                        break;
                    case nameof(Counter.RoundBased):
                        counter.RoundBased = bool.Parse(update.Value);
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.Counters.Remove(_context.Counters.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
