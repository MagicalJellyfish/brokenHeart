using System.Security.Claims;
using brokenHeart.Authentication.Entities;
using brokenHeart.Authentication.Models;

namespace brokenHeart.Authentication.Services
{
    public interface ITokenService
    {
        public TokenResult GenerateTokens(List<Claim> authClaims, Token? token = null);
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string? token);
    }
}
