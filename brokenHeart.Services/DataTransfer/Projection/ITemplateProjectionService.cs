using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection
{
    internal interface ITemplateProjectionService
    {
        public ElementType ProjectionType { get; }

        public ElementList GetTemplateList();
    }
}
