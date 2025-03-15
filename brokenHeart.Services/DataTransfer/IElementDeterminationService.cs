using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer
{
    internal interface IElementDeterminationService
    {
        public ElementType ConvertBaseTypes(ElementType type);

        public ElementParentType ConvertToParentType(ElementType? type);

        public bool IsTemplate(ElementType type);
    }
}
