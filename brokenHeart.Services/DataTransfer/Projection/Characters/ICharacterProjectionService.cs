using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Characters
{
    public interface ICharacterProjectionService
    {
        public List<SimpleCharacter> GetSimpleCharacters(CharacterSearch search);
    }
}
