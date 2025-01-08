using System.ComponentModel.DataAnnotations;

namespace brokenHeart.Auth.Entities
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public ulong DiscordId { get; set; }
        public List<Token> Tokens { get; set; } = new List<Token>();
    }
}
