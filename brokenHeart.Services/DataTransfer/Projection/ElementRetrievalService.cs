using brokenHeart.Models.DataTransfer;
using brokenHeart.Services.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer
{
    public class ElementRetrievalService : IElementRetrievalService
    {
        private readonly IEnumerable<IElementProjectionService> _elementProjectionServices;

        public ElementRetrievalService(
            IEnumerable<IElementProjectionService> elementProjectionServices
        )
        {
            _elementProjectionServices = elementProjectionServices;
        }

        public dynamic? GetElement(ElementType type, int id)
        {
            IElementProjectionService elementProjectionService = GetProjectionService(type);
            return elementProjectionService.GetElement(id);
        }

        private IElementProjectionService GetProjectionService(ElementType type)
        {
            if (type == ElementType.InjuryEffect)
            {
                type = ElementType.Effect;
            }

            return _elementProjectionServices.Single(x => x.ProjectionType == type);
        }
    }
}
