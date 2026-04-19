using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection.Templates
{
    internal class TemplateListProjectionService : ITemplateListProjectionService
    {
        private readonly IEnumerable<ITemplateProjectionService> _templateProjectionServices;

        public TemplateListProjectionService(
            IEnumerable<ITemplateProjectionService> templateProjectionServices
        )
        {
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
                    ElementType.ReminderTemplate,
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
