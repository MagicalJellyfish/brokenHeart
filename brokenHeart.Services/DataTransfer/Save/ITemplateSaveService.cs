using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Save
{
    internal interface ITemplateSaveService
    {
        public ElementType SaveType { get; }

        public void RelateTemplate(int id, ElementParentType parentType, int parentId);

        public void UnrelateTemplate(int id, ElementParentType parentType, int parentId);
    }
}
