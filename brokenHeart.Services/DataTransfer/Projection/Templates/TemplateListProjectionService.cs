using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Templates
{
    internal class TemplateListProjectionService : ITemplateListProjectionService
    {
        private readonly IDaoSearchService _daoSearchService;

        private readonly IEnumerable<ITemplateProjectionService> _templateProjectionServices;

        public TemplateListProjectionService(
            IDaoSearchService daoSearchService,
            IEnumerable<ITemplateProjectionService> templateProjectionServices
        )
        {
            _daoSearchService = daoSearchService;
            _templateProjectionServices = templateProjectionServices;
        }

        public List<ElementList> GetTemplateView()
        {
            List<ElementList> list = new List<ElementList>();
            foreach (
                ElementType type in new List<ElementType>()
                {
                    ElementType.AbilityTemplate,
                    ElementType.TraitTemplate,
                    ElementType.ItemTemplate,
                    ElementType.EffectTemplate,
                    ElementType.CounterTemplate,
                    ElementType.ReminderTemplate
                }
            )
            {
                list.Add(
                    _templateProjectionServices
                        .Single(x => x.ProjectionType == type)
                        .GetTemplateList()
                );
            }

            return list;
        }
    }
}
