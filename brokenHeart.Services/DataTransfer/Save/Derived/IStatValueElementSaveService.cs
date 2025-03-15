using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save.Derived
{
    public interface IStatValueElementSaveService
    {
        public void UpdateStats<T>(int id, List<StatValueModel> stats)
            where T : class;
    }
}
