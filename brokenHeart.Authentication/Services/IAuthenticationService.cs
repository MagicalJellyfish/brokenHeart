using brokenHeart.Authentication.Models;
using brokenHeart.Models;

namespace brokenHeart.Authentication.Services
{
    public interface IAuthenticationService
    {
        public ExecutionResult Register(RegistrationModel registrationModel);
        public ExecutionResult<AuthenticationResult> Login(LoginModel loginModel);
        public void Logout(string username, string token);
        public ExecutionResult<AuthenticationResult> Refresh(RefreshModel refreshModel);
    }
}
