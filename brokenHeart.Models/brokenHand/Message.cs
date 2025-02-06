namespace brokenHeart.Models.brokenHand
{
    public class Message
    {
        public Message(string title, string? description = null, string? color = null)
        {
            Title = title;
            Description = description;
            Color = color;
        }

        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
    }
}
