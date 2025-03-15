using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection.Templates
{
    public interface ITemplateListProjectionService
    {
        public List<ElementList> GetTemplateView();
    }
}
