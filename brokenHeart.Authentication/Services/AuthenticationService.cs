using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using brokenHeart.Authentication.DB;
using brokenHeart.Authentication.Entities;
using brokenHeart.Authentication.Models;
using brokenHeart.Database.DAO;
using brokenHeart.DB;
using brokenHeart.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace brokenHeart.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthDbContext _authContext;
        private readonly BrokenDbContext _brokenContext;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        private readonly string _registrationFile = "registrations.txt";

        public AuthenticationService(
            AuthDbContext authContext,
            BrokenDbContext brokenContext,
            IConfiguration configuration,
            IPasswordService passwordService,
            ITokenService tokenService
        )
        {
            _authContext = authContext;
            _brokenContext = brokenContext;
            _configuration = configuration;
            _passwordService = passwordService;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public ExecutionResult Register(RegistrationModel registrationModel)
        {
            if (registrationModel == null)
            {
                return new ExecutionResult()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Invalid client request"
                };
            }

            bool registrationTokenValid = false;
            List<string> lines = File.ReadAllLines(_registrationFile).ToList();
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
                return new ExecutionResult()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Message = "Registration token invalid"
                };
            }

            bool userExists = _authContext.Users.Any(x =>
                x.Username.ToLower() == registrationModel.Username.ToLower()
            );

            if (userExists)
            {
                return new ExecutionResult()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "User already exists"
                };
            }

            User user =
                new()
                {
                    Username = registrationModel.Username,
                    DiscordId = registrationModel.DiscordId,
                    Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128 / 8))
                };

            ExecutionResult result = _passwordService.ValidatePasswordConstraints(
                registrationModel.Password
            );
            if (!result.Succeeded)
            {
                return result;
            }

            user.HashedPassword = _passwordService.HashPassword(
                registrationModel.Password,
                user.Salt
            );
            _authContext.Users.Add(user);
            _authContext.SaveChanges();

            _brokenContext.UserSimplified.Add(
                new UserSimplified(registrationModel.Username, registrationModel.DiscordId)
            );
            _brokenContext.SaveChanges();

            File.WriteAllLines(_registrationFile, writeLines);

            return new ExecutionResult();
        }

        public ExecutionResult<AuthenticationResult> Login(LoginModel loginModel)
        {
            if (loginModel == null)
            {
                return new ExecutionResult<AuthenticationResult>()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Invalid client request"
                };
            }

            User user = _authContext
                .Users.Include(x => x.Tokens)
                .Single(x => x.Username.ToLower() == loginModel.Username.ToLower());

            if (user == null)
            {
                return new ExecutionResult<AuthenticationResult>()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Message = "Invalid username"
                };
            }

            if (
                !_passwordService.VerifyPassword(
                    user.HashedPassword,
                    loginModel.Password,
                    user.Salt
                )
            )
            {
                return new ExecutionResult<AuthenticationResult>()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Message = "Invalid password"
                };
            }

            List<Claim> authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            TokenResult tokenResult = _tokenService.GenerateTokens(authClaims);

            user.Tokens.Add(tokenResult.Token);
            _authContext.SaveChanges();

            return new ExecutionResult<AuthenticationResult>()
            {
                Value = new AuthenticationResult()
                {
                    Username = user.Username,
                    AccessToken = tokenResult.Token.AccessToken,
                    AccessTokenExpiration = (
                        (DateTimeOffset)tokenResult.AccessTokenExpiryTime
                    ).ToUnixTimeMilliseconds(),
                    RefreshToken = tokenResult.Token.RefreshToken,
                    RefreshTokenExpiration = (
                        (DateTimeOffset)tokenResult.Token.RefreshTokenExpiryTime
                    ).ToUnixTimeMilliseconds()
                }
            };
        }

        public void Logout(string username, string token)
        {
            User user = _authContext
                .Users.Include(x => x.Tokens)
                .Single(x => x.Username.ToLower() == username);

            Token dbToken = user.Tokens.Single(x => x.AccessToken == token);

            _authContext.Tokens.Remove(dbToken);
            _authContext.SaveChanges();
        }

        public ExecutionResult<AuthenticationResult> Refresh(RefreshModel refreshModel)
        {
            if (refreshModel == null)
            {
                return new ExecutionResult<AuthenticationResult>()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Invalid client request"
                };
            }

            string? accessToken = refreshModel.AccessToken;
            string? refreshToken = refreshModel.RefreshToken;

            ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return new ExecutionResult<AuthenticationResult>()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Invalid access token or refresh token"
                };
            }

            User? user = _authContext
                .Users.Include(x => x.Tokens)
                .SingleOrDefault(x => x.Username.ToLower() == principal.Identity.Name.ToLower());

            if (user == null)
            {
                return new ExecutionResult<AuthenticationResult>()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Invalid access token"
                };
            }

            Token dbToken = user.Tokens.Single(x => x.AccessToken == accessToken);

            if (refreshToken != dbToken.RefreshToken)
            {
                _authContext.Tokens.Remove(dbToken);
                _authContext.SaveChanges();

                return new ExecutionResult<AuthenticationResult>()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Invalid refresh token"
                };
            }

            if (
                user.Tokens.Single(x =>
                    x.AccessToken == refreshModel.AccessToken
                ).RefreshTokenExpiryTime <= DateTime.Now
            )
            {
                _authContext.Tokens.Remove(dbToken);

                return new ExecutionResult<AuthenticationResult>()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Refresh token expired"
                };
            }

            TokenResult tokenResult = _tokenService.GenerateTokens(principal.Claims.ToList());

            _authContext.Tokens.Update(tokenResult.Token);
            _authContext.SaveChanges();

            return new ExecutionResult<AuthenticationResult>()
            {
                Value = new AuthenticationResult()
                {
                    Username = user.Username,
                    AccessToken = tokenResult.Token.AccessToken,
                    AccessTokenExpiration = (
                        (DateTimeOffset)tokenResult.AccessTokenExpiryTime
                    ).ToUnixTimeMilliseconds(),
                    RefreshToken = tokenResult.Token.RefreshToken,
                    RefreshTokenExpiration = (
                        (DateTimeOffset)tokenResult.Token.RefreshTokenExpiryTime
                    ).ToUnixTimeMilliseconds()
                }
            };
        }
    }
}
