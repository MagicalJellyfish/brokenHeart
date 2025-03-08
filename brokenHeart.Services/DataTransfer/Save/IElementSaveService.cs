using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save
{
    internal interface IElementSaveService
    {
        public ElementType SaveType { get; }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId);

        public void DeleteElement(int id);
    }
}
