namespace brokenHeart.Authentication.Models
{
    public class AuthenticationResult
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public long AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public long RefreshTokenExpiration { get; set; }
    }
}
