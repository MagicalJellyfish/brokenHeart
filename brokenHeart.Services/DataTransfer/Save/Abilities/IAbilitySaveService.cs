using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Abilities
{
    public interface IAbilitySaveService
    {
        public void UpdateRolls(int id, List<RollModel> stats);
    }
}
