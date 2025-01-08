using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using brokenHeart.Auth;
using brokenHeart.Auth.DB;
using brokenHeart.Auth.Entities;
using brokenHeart.DB;
using brokenHeart.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly BrokenDbContext _brokenDbContext;

        public AuthController(
            AuthDbContext context,
            IConfiguration configuration,
            BrokenDbContext brokenDbContext
        )
        {
            _context = context;
            _configuration = configuration;
            _brokenDbContext = brokenDbContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegistrationModel registrationModel)
        {
            bool registrationTokenValid = false;
            List<string> lines = System.IO.File.ReadAllLines("Auth/registrations.txt").ToList();
            List<string> writeLines = new List<string>();

            foreach (string line in lines)
            {
                string convertedRegistrationToken = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: registrationModel.RegistrationToken,
                        salt: Convert.FromBase64String(_configuration["Registration_Salt"]),
                        prf: KeyDerivationPrf.HMACSHA512,
                        iterationCount: 300000,
                        numBytesRequested: 128 / 8
                    )
                );

                if (line == convertedRegistrationToken)
                {
                    registrationTokenValid = true;
                }
                else
                {
                    writeLines.Add(line);
                }
            }

            if (!registrationTokenValid)
            {
                return Unauthorized("Registration token invalid");
            }

            bool userExists = _context.Users.Any(x =>
                x.Username.ToLower() == registrationModel.Username.ToLower()
            );

            if (userExists)
            {
                return BadRequest("User already exists");
            }

            User user =
                new()
                {
                    Username = registrationModel.Username,
                    DiscordId = registrationModel.DiscordId,
                    Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128 / 8))
                };

            ExecutionResult result = AuthFunctions.ValidatePasswordConstraints(
                registrationModel.Password
            );
            if (!result.Succeeded)
            {
                return BadRequest(result.Message);
            }

            user.HashedPassword = AuthFunctions.HashPassword(registrationModel.Password, user.Salt);

            _context.Users.Add(user);
            _context.SaveChanges();

            _brokenDbContext.UserSimplified.Add(
                new UserSimplified(registrationModel.Username, registrationModel.DiscordId)
            );
            _brokenDbContext.SaveChanges();

            System.IO.File.WriteAllLines("Auth/registrations.txt", writeLines);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            User user = _context
                .Users.Include(x => x.Tokens)
                .Single(x => x.Username.ToLower() == loginModel.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            if (!AuthFunctions.VerifyPassword(user.HashedPassword, loginModel.Password, user.Salt))
            {
                return Unauthorized("Invalid password");
            }

            List<Claim> authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            JwtSecurityToken accessToken = GenerateAccessToken(authClaims);
            string writtenAccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken);
            string refreshToken = GenerateRefreshToken();
            int.TryParse(
                _configuration["JWT:RefreshTokenValidityInDays"],
                out int refreshTokenValidityInDays
            );

            Token token = new Token()
            {
                AccessToken = writtenAccessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays)
            };

            user.Tokens.Add(token);
            _context.SaveChanges();

            return Ok(
                new
                {
                    Username = user.Username,
                    AccessToken = token.AccessToken,
                    AccessTokenExpiration = (
                        (DateTimeOffset)accessToken.ValidTo
                    ).ToUnixTimeMilliseconds(),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = (
                        (DateTimeOffset)token.RefreshTokenExpiryTime
                    ).ToUnixTimeMilliseconds()
                }
            );
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            User user = _context
                .Users.Include(x => x.Tokens)
                .Single(x => x.Username.ToLower() == User.Identity.Name.ToLower());

            Token token = user.Tokens.Single(x =>
                x.AccessToken
                == Request.Headers.Single(y => y.Key == "Authorization").Value.Single().Substring(7)
            );

            _context.Tokens.Remove(token);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshModel tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            User user = _context
                .Users.Include(x => x.Tokens)
                .Single(x => x.Username.ToLower() == principal.Identity.Name.ToLower());

            if (user == null)
            {
                return BadRequest("Invalid access token");
            }

            Token dbToken = user.Tokens.Single(x => x.AccessToken == accessToken);

            if (refreshToken != dbToken.RefreshToken)
            {
                _context.Tokens.Remove(dbToken);
                _context.SaveChanges();
                return BadRequest("Invalid refresh token");
            }

            if (
                user.Tokens.Single(x =>
                    x.AccessToken == tokenModel.AccessToken
                ).RefreshTokenExpiryTime <= DateTime.Now
            )
            {
                _context.Tokens.Remove(dbToken);
                return BadRequest("Refresh token expired");
            }

            JwtSecurityToken newAccessToken = GenerateAccessToken(principal.Claims.ToList());
            string writtenNewAccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken);
            string newRefreshToken = GenerateRefreshToken();

            int.TryParse(
                _configuration["JWT:RefreshTokenValidityInDays"],
                out int refreshTokenValidityInDays
            );

            dbToken.AccessToken = writtenNewAccessToken;
            dbToken.RefreshToken = newRefreshToken;
            dbToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

            _context.Tokens.Update(dbToken);
            _context.SaveChanges();

            return new ObjectResult(
                new
                {
                    Username = user.Username,
                    AccessToken = writtenNewAccessToken,
                    AccessTokenExpiration = (
                        (DateTimeOffset)newAccessToken.ValidTo
                    ).ToUnixTimeMilliseconds(),
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiration = (
                        (DateTimeOffset)dbToken.RefreshTokenExpiryTime
                    ).ToUnixTimeMilliseconds()
                }
            );
        }

        [HttpGet("discord")]
        [Authorize]
        public async Task<ActionResult<object>> GetId()
        {
            var user = _context.Users.Single(x =>
                x.Username.ToLower() == User.Identity.Name.ToLower()
            );

            if (user == null)
            {
                return NotFound("User not found!");
            }

            return new { discordId = user.DiscordId.ToString() };
        }

        [HttpPatch("discord/{discordId}")]
        [Authorize]
        public async Task<ActionResult> ChangeId(ulong discordId)
        {
            var user = _context.Users.Single(x =>
                x.Username.ToLower() == User.Identity.Name.ToLower()
            );

            if (user == null)
            {
                return NotFound("User not found!");
            }

            user.DiscordId = discordId;
            _context.Users.Update(user);
            _context.SaveChanges();

            UserSimplified userSimple = _brokenDbContext.UserSimplified.Single(x =>
                x.Username == user.Username
            );
            userSimple.DiscordId = discordId;
            _brokenDbContext.SaveChanges();

            return NoContent();
        }

        private JwtSecurityToken GenerateAccessToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT_Secret"])
            );
            int tokenValidityInMinutes = int.Parse(
                _configuration["JWT:AccessTokenValidityInMinutes"]
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    authSigningKey,
                    SecurityAlgorithms.HmacSha512
                )
            );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT_Secret"])
                ),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken
            );

            if (
                securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha512,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
