namespace brokenHeart.Database.Interfaces
{
    public interface IOrderableElement : IElement
    {
        public int ViewPosition { get; set; }
    }
}
