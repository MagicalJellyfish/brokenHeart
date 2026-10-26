using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.DB;
using brokenHeart.Services.DataTransfer.Save.Entities;
using LinqKit;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Items
{
    internal class ItemTransferService : IItemTransferService
    {
        private readonly BrokenDbContext _context;

        public ItemTransferService(BrokenDbContext context)
        {
            _context = context;
        }

        public int TransferItem(int id, int characterId, int amount)
        {
            Item item = _context.Items.Single(x => x.Id == id);

            if (!_context.Characters.Any(x => x.Id == characterId))
            {
                throw new Exception($"Character {characterId} does not exist");
            }

            if (item.CharacterId == characterId && amount == item.Amount)
            {
                return item.Id;
            }

            if (amount <= 0 || amount > item.Amount)
            {
                throw new Exception($"Amount must be between 1 and {item.Amount}");
            }

            if (amount == item.Amount)
            {
                item.CharacterId = characterId;
                _context.SaveChanges();

                return item.Id;
            }

            Item duplicate = _context
                .Items.Where(x => x.Id == id)
                .Select(x => Duplication.DuplicateItem.Invoke(x))
                .Single();

            duplicate.CharacterId = characterId;
            duplicate.Amount = amount;
            item.Amount -= amount;

            _context.Items.Add(duplicate);
            _context.SaveChanges();

            return duplicate.Id;
        }
    }
}
