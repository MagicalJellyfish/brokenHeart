using Microsoft.AspNetCore.Identity;

namespace brokenHeart.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public ulong DiscordId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
