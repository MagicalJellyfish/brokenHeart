using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save
{
    public interface IElementSubmissionService
    {
        public ExecutionResult<int> CreateElement(
            ElementType type,
            ElementParentType parentType,
            int parentId
        );

        public void DeleteElement(ElementType type, int id);
    }
}
