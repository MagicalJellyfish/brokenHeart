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
    [Route("[controller]")]
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
        public ActionResult Register(RegistrationModel registrationModel)
        {
            ExecutionResult result = _authenticationService.Register(registrationModel);
            return StatusCode((int)result.StatusCode, result.Message);
        }

        [HttpPost("login")]
        public ActionResult Login(LoginModel loginModel)
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
        public ActionResult Logout()
        {
            string? token = Request
                .Headers.SingleOrDefault(y => y.Key == "Authorization")
                .Value.SingleOrDefault()
                ?.Substring(7);

            if (token == null)
            {
                return BadRequest("No token supplied");
            }

            _authenticationService.Logout(User.Identity.Name.ToLower(), token);

            return Ok();
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(RefreshModel refreshModel)
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
        public ActionResult<object> GetId()
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
        public ActionResult ChangeId(ulong discordId)
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
