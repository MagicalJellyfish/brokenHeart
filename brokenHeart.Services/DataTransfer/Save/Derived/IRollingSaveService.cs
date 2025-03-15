using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Auxiliary
{
    public interface IRollingSaveService
    {
        public void UpdateRolls<T>(int id, List<RollModel> rolls)
            where T : class;
    }
}
