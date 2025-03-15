using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection
{
    public interface IElementRetrievalService
    {
        public dynamic? GetElement(ElementType type, int id);

        public ElementList GetTemplateSelection(ElementType type);
    }
}
