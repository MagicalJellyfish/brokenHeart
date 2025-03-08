using brokenHeart.Database.DAO;
using brokenHeart.Models.DataTransfer.Search.Characters;

namespace brokenHeart.Services.DataTransfer.Search.Characters
{
    public interface ICharacterSearchService
    {
        public IQueryable<Character> GetCharacters(CharacterSearch search);
    }
}
