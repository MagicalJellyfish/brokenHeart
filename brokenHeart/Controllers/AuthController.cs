﻿using brokenHeart.Auth;
using brokenHeart.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly BrokenDbContext _brokenDbContext;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, BrokenDbContext brokenDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _brokenDbContext = brokenDbContext;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegistrationModel registerModel)
        {
            var userExists = await _userManager.FindByNameAsync(registerModel.Username);

            if (userExists != null)
            {
                return BadRequest("User already exists!");
            }

            ApplicationUser user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.Username,
                DiscordId = (ulong)registerModel.DiscordId
            };

            var createUserResult = await _userManager.CreateAsync(user, registerModel.Password);

            if (!createUserResult.Succeeded)
            {
                return StatusCode(500, "User creation failed! " + createUserResult.Errors.First().Description);
            }

            string role = UserRoles.Unverified;
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                await _userManager.AddToRoleAsync(user, role);
            }

            _brokenDbContext.UserSimplified.Add(new Entities.UserSimplified(registerModel.Username, (ulong)registerModel.DiscordId));
            _brokenDbContext.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            if (!await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                return Unauthorized("Invalid password");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateAccessToken(authClaims);
            var refreshToken = GenerateRefreshToken();

            int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Username = user.UserName,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiration = ((DateTimeOffset)token.ValidTo).ToUnixTimeMilliseconds(),
                RefreshToken = refreshToken,
                RefreshTokenExpiration = ((DateTimeOffset)user.RefreshTokenExpiryTime).ToUnixTimeMilliseconds()
            });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            if(refreshToken != user.RefreshToken)
            {
                user.RefreshToken = null;
                return BadRequest("Invalid refresh token");
            }

            var newAccessToken = GenerateAccessToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                Username = user.UserName,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                AccessTokenExpiration = ((DateTimeOffset)newAccessToken.ValidTo).ToUnixTimeMilliseconds(),
                RefreshToken = newRefreshToken,
                RefreshTokenExpiration = ((DateTimeOffset)user.RefreshTokenExpiryTime).ToUnixTimeMilliseconds()
            });
        }

        [HttpGet]
        [Route("discord")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<object>> GetId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            return new { discordId = user.DiscordId.ToString() };
        }

        [HttpPatch]
        [Route("discord/{discordId}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult> ChangeId(ulong discordId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            user.DiscordId = discordId;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpPatch]
        [Route("roles/{name}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> EditRoles(string name, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByNameAsync(name);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            IEnumerable<string> appliedRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return NotFound("Role does not exist!");
                }

                if (appliedRoles.Contains(role))
                {
                    break;
                }

                await _userManager.AddToRoleAsync(user, role);
            }

            foreach (string appliedRole in appliedRoles)
            {
                if (!roles.Contains(appliedRole))
                {
                    await _userManager.RemoveFromRoleAsync(user, appliedRole);
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("users")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> GetUsers()
        {
            try
            {
                var users = _userManager.Users;

                if (users == null)
                {
                    return BadRequest("No users found!");
                }

                return Ok(users.Select(x => x.UserName));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("roles/{name}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> GetRoles(string name)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(name);

                if(user == null)
                {
                    return BadRequest("User not found!");
                }

                return Ok(await _userManager.GetRolesAsync(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        private JwtSecurityToken GenerateAccessToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            int.TryParse(_configuration["JWT:AccessTokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512)
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
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        [HttpGet]
        [Route("test")]
        [Authorize(Roles = UserRoles.Unverified)]
        public async Task<ActionResult> Test()
        {
            return Ok(User.Identity.Name);
        }
    }
}