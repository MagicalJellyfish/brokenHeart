using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save
{
    internal class ElementSubmissionService : IElementSubmissionService
    {
        private readonly IEnumerable<IElementSaveService> _elementSaveServices;

        public ElementSubmissionService(IEnumerable<IElementSaveService> elementSaveServices)
        {
            _elementSaveServices = elementSaveServices;
        }

        public ExecutionResult<int> CreateElement(
            ElementType type,
            ElementParentType parentType,
            int parentId
        )
        {
            IElementSaveService elementSaveService = GetSaveService(type);
            return elementSaveService.CreateElement(parentType, parentId);
        }

        public void ReorderElements(ElementType type, List<ElementReorder> reorders)
        {
            IElementSaveService elementSaveService = GetSaveService(type);
            elementSaveService.ReorderElements(reorders);
        }

        public void UpdateElement(ElementType type, int id, List<ElementUpdate> updates)
        {
            IElementSaveService elementSaveService = GetSaveService(type);
            elementSaveService.UpdateElement(id, updates);
        }

        public void DeleteElement(ElementType type, int id)
        {
            IElementSaveService elementSaveService = GetSaveService(type);
            elementSaveService.DeleteElement(id);
        }

        private IElementSaveService GetSaveService(ElementType type)
        {
            if (type == ElementType.InjuryEffect)
            {
                type = ElementType.Effect;
            }

            return _elementSaveServices.Single(x => x.SaveType == type);
        }
    }
}
