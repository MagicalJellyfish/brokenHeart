using brokenHeart.Models.Rolling;

namespace brokenHeart.Services.Rolling
{
    public interface ICharacterRollService
    {
        public RollResult CharRollString(string input, int charId);
        public List<RollResult> CharRollString(string input, int charId, int repeat = 1);
    }
}
