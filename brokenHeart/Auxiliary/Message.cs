namespace brokenHeart.Auxiliary
{
    public class Message
    {
        public Message(string title, string? description = null)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
