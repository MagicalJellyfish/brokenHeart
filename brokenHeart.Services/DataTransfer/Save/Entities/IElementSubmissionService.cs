using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save.Entities
{
    public interface IElementSubmissionService
    {
        public ExecutionResult<int> CreateElement(
            ElementType type,
            ElementParentType parentType,
            int parentId
        );

        public void ReorderElements(ElementType type, List<ElementReorder> reorders);

        public void UpdateElement(ElementType type, int id, List<ElementUpdate> updates);

        public void DeleteElement(ElementType type, int id);
    }
}
