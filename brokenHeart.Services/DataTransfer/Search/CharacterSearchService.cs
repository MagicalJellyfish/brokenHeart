using brokenHeart.Database.DAO;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search
{
    internal class CharacterSearchService : ICharacterSearchService
    {
        private readonly BrokenDbContext _context;

        public CharacterSearchService(BrokenDbContext context)
        {
            _context = context;
        }

        public IQueryable<Character> GetCharacters(CharacterSearch search)
        {
            IQueryable<Character> characters = _context.Characters;

            if (search.Id != null)
            {
                characters = characters.Where(x => x.Id == search.Id);
            }

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

        public IQueryable<Character> GetSingleCharacter(CharacterSearch search)
        {
            IQueryable<Character> characters = GetCharacters(search);

            if (characters.Count() < 1 || characters.Count() > 1)
            {
                throw new Exception("Found too many elements for search parameters!");
            }

            return characters;
        }
    }
}
