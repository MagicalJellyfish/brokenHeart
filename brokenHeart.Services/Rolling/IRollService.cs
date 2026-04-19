using brokenHeart.Models.Rolling;

namespace brokenHeart.Services.Rolling
{
    public interface IRollService
    {
        public RollResult RollString(string input, string? original = null);
        public List<RollResult> RollString(string input, string? original = null, int repeat = 1);
        public RollResult Roll(int rolls, int die, KeepType keep = KeepType.None, int keepNum = -1);
    }
}
