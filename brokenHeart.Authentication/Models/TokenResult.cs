using brokenHeart.Authentication.Entities;

namespace brokenHeart.Authentication.Models
{
    public class TokenResult
    {
        public Token Token { get; set; }
        public DateTime AccessTokenExpiryTime { get; set; }
    }
}
