namespace brokenHeart.Models.Utility
{
    public class Children<T>
    {
        public List<T> Instances { get; set; } = new List<T>();
        public List<T> Flat { get; set; } = new List<T>();
    }
}
