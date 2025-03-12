using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save.Auxiliary
{
    public interface IOrderableSaveService
    {
        public void ReorderElements<T>(List<ElementReorder> reorders)
            where T : class;
    }
}
