using brokenHeart.Models;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save.Characters
{
    public interface ICharacterSaveService
    {
        public ExecutionResult<int> CreateCharacter(string username);

        public ExecutionResult PatchCharacter(int id, List<CharacterPatch> patches);

        public void DeleteCharacter(int id);
    }
}
