﻿using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Items
{
    internal class ItemSaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Item;

        private readonly BrokenDbContext _context;
        private readonly IModifierSaveService _modifierSaveService;

        public ItemSaveService(BrokenDbContext context, IModifierSaveService modifierSaveService)
        {
            _context = context;
            _modifierSaveService = modifierSaveService;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
        {
            Item item = new Item();

            switch (parentType)
            {
                case ElementParentType.Character:
                    item.CharacterId = parentId;
                    break;
                default:
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Items.Add(item);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = item.Id };
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            List<Item> items = _context
                .Items.Where(x => reorders.Select(y => y.Id).Contains(x.Id))
                .ToList();

            foreach (Item item in items)
            {
                item.ViewPosition = reorders.Single(x => x.Id == item.Id).ViewPosition;
            }

            _context.SaveChanges();
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Item item = _context.Items.Single(x => x.Id == id);

            _modifierSaveService.UpdateGivenModifier(item, updates);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Item.Equipped):
                        item.Equipped = bool.Parse(update.Value);
                        break;
                    case nameof(Item.Amount):
                        item.Amount = update.Value.SafeParseInt();
                        break;
                    case nameof(Item.Unit):
                        item.Unit = update.Value;
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.Items.Remove(_context.Items.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
