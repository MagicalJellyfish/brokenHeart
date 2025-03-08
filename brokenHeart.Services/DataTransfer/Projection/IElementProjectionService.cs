using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer.Projection
{
    public interface IElementProjectionService
    {
        public ElementType ProjectionType { get; }

        public dynamic GetElement(int id);
    }
}
