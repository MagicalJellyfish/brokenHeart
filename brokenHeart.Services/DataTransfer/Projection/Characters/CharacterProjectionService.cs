using brokenHeart.Database.DAO;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search.Characters;

namespace brokenHeart.Services.DataTransfer.Projection.Characters
{
    internal class CharacterProjectionService : ICharacterProjectionService
    {
        private readonly ICharacterSearchService _characterSearchService;

        public CharacterProjectionService(ICharacterSearchService characterSearchService)
        {
            _characterSearchService = characterSearchService;
        }

        public List<SimpleCharacter> GetSimpleCharacters(CharacterSearch search)
        {
            IQueryable<Character> characters = _characterSearchService.GetCharacters(search);

            return characters.Select(SimpleCharacter.Map).ToList();
        }
        }
    }
}
