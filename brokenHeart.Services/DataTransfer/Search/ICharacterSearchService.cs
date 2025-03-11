using brokenHeart.Database.DAO;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search
{
    public interface ICharacterSearchService
    {
        public IQueryable<Character> GetCharacters(CharacterSearch search);
        public IQueryable<Character> GetSingleCharacter(CharacterSearch search);
    }
}
