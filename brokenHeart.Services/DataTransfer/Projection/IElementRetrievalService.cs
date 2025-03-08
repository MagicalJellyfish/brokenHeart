using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Projection
{
    public interface IElementRetrievalService
    {
        public dynamic? GetElement(ElementType type, int id);
    }
}
