using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save
{
    internal interface IElementSaveService
    {
        public ElementType SaveType { get; }

        public int CreateElement(ElementParentType parentType, int? parentId);

        public void ReorderElements(List<ElementReorder> reorders);

        public void UpdateElement(int id, List<ElementUpdate> updates);

        public void DeleteElement(int id);
    }
}
