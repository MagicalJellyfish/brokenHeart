namespace brokenHeart.Database.Interfaces
{
    internal interface IOrderableElement : IElement
    {
        public int ViewPosition { get; set; }
    }
}
