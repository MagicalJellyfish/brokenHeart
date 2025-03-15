using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save
{
    public interface IElementSubmissionService
    {
        public int CreateElement(ElementType type, ElementType? parentType, int? parentId);

        public void ReorderElements(ElementType type, List<ElementReorder> reorders);

        public void UpdateRolls(ElementType type, int id, List<RollModel> rolls);

        public void UpdateStats(ElementType type, int id, List<StatValueModel> stats);

        public void UpdateElement(ElementType type, int id, List<ElementUpdate> updates);

        public void DeleteElement(ElementType type, int id);

        public void RelateTemplate(ElementType type, int id, ElementType parentType, int parentId);

        public void UnrelateTemplate(
            ElementType type,
            int id,
            ElementType parentType,
            int parentId
        );

        public int InstantiateTemplate(
            ElementType type,
            int id,
            ElementType parentType,
            int parentId
        );
    }
}
