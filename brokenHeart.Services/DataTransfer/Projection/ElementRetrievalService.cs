using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer
{
    internal class ElementRetrievalService : IElementRetrievalService
    {
        private readonly IElementDeterminationService _elementDeterminationService;
        private readonly IEnumerable<IElementProjectionService> _elementProjectionServices;
        private readonly IEnumerable<ITemplateProjectionService> _templateProjectionServices;

        public ElementRetrievalService(
            IElementDeterminationService elementDeterminationService,
            IEnumerable<IElementProjectionService> elementProjectionServices,
            IEnumerable<ITemplateProjectionService> templateProjectionServices
        )
        {
            _elementDeterminationService = elementDeterminationService;
            _elementProjectionServices = elementProjectionServices;
            _templateProjectionServices = templateProjectionServices;
        }

        public dynamic? GetElement(ElementType type, int id)
        {
            IElementProjectionService elementProjectionService = GetElementProjectionService(type);
            return elementProjectionService.GetElement(id);
        }

        public ElementList GetTemplateSelection(ElementType type)
        {
            ITemplateProjectionService templateProjectionService = GetTemplateProjectionService(
                type
            );

            return templateProjectionService.GetTemplateList();
        }

        private IElementProjectionService GetElementProjectionService(ElementType type)
        {
            type = _elementDeterminationService.ConvertBaseTypes(type);

            return _elementProjectionServices.Single(x => x.ProjectionType == type);
        }

        private ITemplateProjectionService GetTemplateProjectionService(ElementType type)
        {
            type = _elementDeterminationService.ConvertBaseTypes(type);

            if (!_elementDeterminationService.IsTemplate(type))
            {
                throw new Exception("Type is not a Template");
            }

            return _templateProjectionServices.Single(x => x.ProjectionType == type);
        }
    }
}
