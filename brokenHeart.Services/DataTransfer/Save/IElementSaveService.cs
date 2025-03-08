using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save
{
    internal interface IElementSaveService
    {
        public ElementType SaveType { get; }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId);

        public void UpdateElement(int id, List<ElementUpdate> updates);

        public void DeleteElement(int id);
    }
}
