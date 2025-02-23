using brokenHeart.Database.DAO;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search.Characters
{
    internal class CharacterSearchService : SearchService, ICharacterSearchService
    {
        public CharacterSearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<Character> GetCharacters(CharacterSearch search)
        {
            IQueryable<Character> characters = _context.Characters;

            if (search.Name != null)
            {
                characters = characters.Where(x => x.Name.Contains(search.Name));
            }

            if (search.IsNpc != null)
            {
                characters = characters.Where(x => x.IsNPC == search.IsNpc);
            }

            if (search.OwnedBy != null)
            {
                characters = characters.Where(x => x.Owner.Username == search.OwnedBy);
            }

            if (search.NotOwnedBy != null)
            {
                characters = characters.Where(x => x.Owner.Username != search.NotOwnedBy);
            }

            return characters;
        }
    }
}
