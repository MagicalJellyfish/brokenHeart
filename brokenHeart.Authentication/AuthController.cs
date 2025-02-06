using brokenHeart.Authentication.DB;
using brokenHeart.Authentication.Models;
using brokenHeart.Authentication.Services;
using brokenHeart.Database.DAO;
using brokenHeart.DB;
using brokenHeart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly BrokenDbContext _brokenDbContext;
        private readonly IAuthenticationService _authenticationService;

        public AuthController(
            AuthDbContext context,
            BrokenDbContext brokenDbContext,
            IAuthenticationService authenticationService
        )
        {
            _context = context;
            _brokenDbContext = brokenDbContext;
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegistrationModel registrationModel)
        {
            ExecutionResult result = _authenticationService.Register(registrationModel);
            return StatusCode((int)result.StatusCode, result.Message);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            ExecutionResult<AuthenticationResult> result = _authenticationService.Login(loginModel);

            if (!result.Succeeded)
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }

            return Ok(result.Value);
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            _authenticationService.Logout(
                User.Identity.Name.ToLower(),
                Request.Headers.Single(y => y.Key == "Authorization").Value.Single().Substring(7)
            );

            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshModel refreshModel)
        {
            ExecutionResult<AuthenticationResult> result = _authenticationService.Refresh(
                refreshModel
            );

            if (!result.Succeeded)
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }

            return Ok(result.Value);
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
    }
}
